using System;
using System.IO;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Zafiro.Core.Files;
using Zafiro.Core.Mixins;
using Zafiro.Core.ProgressReporting;

namespace Zafiro.Core.Downloads
{
    public class Downloader : IDownloader
    {
        private readonly HttpClient client;

        public Downloader(HttpClient client)
        {
            this.client = client;
        }

        public async Task Download(string url, IZafiroFile destination, IOperationProgress progressObserver = null,
            int timeout = 30)
        {
            using var fileStream = await destination.OpenForWrite();
            await Download(url, fileStream, progressObserver, timeout);
        }

        private async Task Download(string url, Stream destination, IOperationProgress progressObserver = null,
            int timeout = 30)
        {
            long? totalBytes = 0;
            ulong bytesWritten = 0;

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
                    bytesWritten += (ulong) bytes.Length;
                    if (totalBytes.HasValue)
                    {
                        progressObserver?.Send(new Percentage((double) bytesWritten / totalBytes.Value));
                    }
                    else
                    {
                        progressObserver.Send(new AbsoluteProgress<ulong>(bytesWritten));
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

        private static readonly int BufferSize = 8 * 1024;

        public async Task<Stream> GetStream(string url, IOperationProgress progress = null, int timeout = 30)
        {
            var tmpFile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            var stream = File.Create(tmpFile, BufferSize, FileOptions.DeleteOnClose);

            await Download(url, stream, progress, timeout);
            stream.Position = 0;
            return stream;
        }
    }
}