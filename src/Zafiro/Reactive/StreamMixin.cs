using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Zafiro.Reactive;

public static class StreamMixin
{
    public static IObservable<Result> DumpTo(this IObservable<byte> source, Stream output, TimeSpan? chunkReadTimeout = default, IScheduler? scheduler = default, int bufferSize = 4096)
    {
        scheduler ??= Scheduler.Default;
        chunkReadTimeout ??= TimeSpan.FromDays(1);

        return source
            .Buffer(bufferSize)
            .Select(chunk => chunk.ToArray())
            .DumpTo(output, chunkReadTimeout, scheduler, bufferSize);
    }

    public static IObservable<Result> DumpTo(this IObservable<byte[]> source, Stream output, TimeSpan? chunkReadTimeout = default, IScheduler? scheduler = default, int bufferSize = 4096)
    {
        scheduler ??= Scheduler.Default;
        chunkReadTimeout ??= TimeSpan.FromDays(1);

        return source
            .Select(chunk => Observable.FromAsync(ct => Result.Try(() => output.WriteAsync(chunk.ToArray(), 0, chunk.Length, ct)), scheduler).Timeout(chunkReadTimeout.Value, scheduler))
            .Concat();
    }

    public static async Task<string> ReadToEnd(this Stream stream, Encoding? encoding = null)
    {
        using var reader = new StreamReader(stream, encoding ?? Encoding.Default);
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    public static async Task<byte[]> ReadBytes(this Stream stream, CancellationToken ct = default)
    {
        int read;
        var buffer = new byte[stream.Length];
        var receivedBytes = 0;

        while ((read = await stream.ReadAsync(buffer, receivedBytes, buffer.Length, ct).ConfigureAwait(false)) < receivedBytes)
        {
            receivedBytes += read;
        }

        return buffer;
    }
    
    public static IObservable<byte[]> ToObservableChunked(this Stream stream, int bufferSize = 4096)
    {
        return Observable.Create<byte[]>(async (observer, cancellationToken) =>
        {
            try
            {
                var buffer = new byte[bufferSize];
                int bytesRead;
                do
                {
                    bytesRead = await stream.ReadAsync(buffer).ConfigureAwait(false);
                    if (bytesRead > 0)
                    {
                        observer.OnNext(buffer[..bytesRead]);
                    }
                } while (bytesRead > 0);
                observer.OnCompleted();
            }
            catch (Exception exception)
            {
                Debugger.Launch();
                observer.OnError(exception);
            }
        });
    }
    
    public static IObservable<byte> ToObservable(this Stream stream, int bufferSize = 4096)
    {
        return Observable.Create<byte>(async (s, ct) =>
        {
            try
            {
                var buffer = new byte[bufferSize];

                int readBytes;
                do
                {
                    readBytes = await stream.ReadAsync(buffer, ct).ConfigureAwait(false);
                    for (var i = 0; i < readBytes; i++)
                    {
                        s.OnNext(buffer[i]);
                    }
                } while (readBytes > 0);

                s.OnCompleted();
            }
            catch (Exception e)
            {
                s.OnError(e);
            }
        });
    }
}