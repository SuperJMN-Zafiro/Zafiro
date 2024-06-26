﻿using System;
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
                var stream = await streamTaskFactory().ConfigureAwait(false);
                await using var stream1 = stream.ConfigureAwait(false);
                await ProcessStream(observer, cancellationToken, stream, bufferSize).ConfigureAwait(false);
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
                var stream = streamFactory();
                await using var stream1 = stream.ConfigureAwait(false);
                await ProcessStream(observer, cancellationToken, stream, bufferSize).ConfigureAwait(false);
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