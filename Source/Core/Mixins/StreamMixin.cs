﻿using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Zafiro.Core.Mixins
{
    public static class StreamMixin
    {
        public static async Task<string> ReadToEnd(this Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public static async Task<byte[]> ReadBytes(this Stream stream)
        {
            int read;
            var buffer = new byte[stream.Length];
            int receivedBytes = 0;

            while ((read = await stream.ReadAsync(buffer, receivedBytes, buffer.Length)) < receivedBytes)
            {
                receivedBytes += read;
            }
            
            return buffer;
        }

        public static IObservable<byte> ToObservable(this Stream stream, int bufferSize = 4096)
        {
            var buffer = new byte[bufferSize];

            return Observable
                .FromAsync(async ct => (bytesRead: await stream.ReadAsync(buffer, 0, buffer.Length, ct), buffer), Scheduler.Immediate)
                .Repeat()
                .TakeWhile(x => x.bytesRead != 0)	
                .Select(x => x.buffer)
                .SelectMany(x => x);
        }

        public static IObservable<byte> ToObservableCustom(this Stream stream, int bufferSize = 4096)
        {
            return Observable.Create<byte>(async (s, ct) =>
            {
                try
                {
                    var buffer = new byte[bufferSize];
                    while (await stream.ReadAsync(buffer, ct) > 0 && !ct.IsCancellationRequested)
                    {
                        for (var i = 0; i < bufferSize; i++)
                        {
                            s.OnNext(buffer[i]);
                        }
                    }
                    s.OnCompleted();
                }
                catch (Exception e)
                {
                    s.OnError(e);
                }
            });
        }
    }
}