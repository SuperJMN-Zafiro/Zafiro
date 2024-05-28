using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Zafiro.DataModel;

public static class ReactiveData
{
    public static IObservable<byte[]> Chunked(this Func<Task<Stream>> streamTaskFactory, int bufferSize = 4096)
    {
        return Observable.Create<byte[]>(async (observer, cancellationToken) =>
        {
            try
            {
                await using var stream = await streamTaskFactory();
                await ProcessStream(observer, cancellationToken, stream, bufferSize);
            }
            catch (Exception exception)
            {
                observer.OnError(exception);
            }
        });
    }
    public static IObservable<byte[]> Chunked(this Func<Stream> streamFactory, int bufferSize = 4096)
    {
        return Observable.Create<byte[]>(async (observer, cancellationToken) =>
        {
            try
            {
                await using var stream = streamFactory();
                await ProcessStream(observer, cancellationToken, stream, bufferSize);
            }
            catch (Exception exception)
            {
                observer.OnError(exception);
            }
        });
    }
    
    private static async Task ProcessStream(IObserver<byte[]> observer, CancellationToken cancellationToken, Stream stream, int bufferSize)
    {
        var buffer = new byte[bufferSize];
        int bytesRead;
        do
        {
            bytesRead = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
            if (bytesRead > 0)
            {
                observer.OnNext(buffer[..bytesRead]);
            }
        } 
        while (bytesRead > 0);
        observer.OnCompleted();
    }
}