using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zafiro.IO;

public static class Ext
{
    public static IObservable<Unit> DumpTo(this IObservable<byte> source, Stream output, TimeSpan? chunkReadTimeout = default, IScheduler? scheduler = default, int bufferSize = 4096)
    {
        scheduler ??= Scheduler.Default;
        chunkReadTimeout ??= TimeSpan.FromDays(1);

        return source
            .Buffer(bufferSize)
            .Select(chunk => Observable.FromAsync(ct => output.WriteAsync(chunk.ToArray(), 0, chunk.Count, ct), scheduler).Timeout(chunkReadTimeout.Value, scheduler))
            .Concat()
            .LastAsync();
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

    /// <summary>
    /// This has caused some issues in the past.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    public static IObservable<byte> ToObservableAlternate(this Stream stream, int bufferSize = 4096)
    {
        var buffer = new byte[bufferSize];

        return Observable
            .FromAsync(async ct => (bytesRead: await stream.ReadAsync(buffer, 0, buffer.Length, ct).ConfigureAwait(false), buffer))
            .Repeat()
            .TakeWhile(x => x.bytesRead != 0)
            .Select(x => x.buffer)
            .SelectMany(x => x);
    }
}