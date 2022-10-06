using System;
using System.IO;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Core;
using Core.Mixins;
using Core.ProgressReporting;
using FileSystem;

namespace Network
{
    public class Downloader : IDownloader
    {
        private const int BufferSize = 8 * 1024;
        private readonly IHttpClientFactory httpClientFactoryFactory;

        public Downloader(IHttpClientFactory httpClientFactory)
        {
            httpClientFactoryFactory = httpClientFactory;
        }

        public async Task Download(Uri uri, IZafiroFile destination, IOperationProgress progressObserver = null,
            int timeout = 30)
        {
            await using var fileStream = await destination.OpenWrite();
            await Download(uri, fileStream, progressObserver, timeout);
        }

        public async Task<Stream> GetStream(Uri url, IOperationProgress progress = null, int timeout = 30)
        {
            var tmpFile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            var stream = File.Create(tmpFile, BufferSize, FileOptions.DeleteOnClose);

            await Download(url, stream, progress, timeout);
            stream.Position = 0;
            return stream;
        }

        private async Task Download(Uri url, Stream destination, IOperationProgress progressObserver = null,
            int timeout = 30)
        {
            long? totalBytes = 0;
            ulong bytesWritten = 0;

            var client = httpClientFactoryFactory.CreateClient();

            await ObservableMixin.Using(() => client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead),
                    s =>
                    {
                        totalBytes = s.Content.Headers.ContentLength;
                        if (!totalBytes.HasValue)
                        {
                            progressObserver?.Send(new Unknown());
                        }

                        return ObservableMixin.Using(() => s.Content.ReadAsStreamAsync(),
                            contentStream => contentStream.ReadToEndObservable());
                    })
                .Do(bytes =>
                {
                    bytesWritten += (ulong)bytes.Length;
                    if (totalBytes.HasValue)
                    {
                        progressObserver?.Send(new Percentage((double)bytesWritten / totalBytes.Value));
                    }
                    else
                    {
                        progressObserver?.Send(new AbsoluteProgress<ulong>(bytesWritten));
                    }
                })
                .Timeout(TimeSpan.FromSeconds(timeout))
                .Select(bytes => Observable.FromAsync(async () =>
                {
                    await destination.WriteAsync(bytes, 0, bytes.Length);
                    return Unit.Default;
                }))
                .Merge(1);

            progressObserver?.Send(new Done());
        }
    }
}