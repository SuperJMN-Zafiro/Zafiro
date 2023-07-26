using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zafiro.Core.Mixins;

namespace Core.Tests;

public class ProgressTests
{
    [Fact]
    public async Task Test()
    {
        var buffer = new byte[] { 65 };
        var inputStream = new MemoryStream(buffer).ToObservable();
        var memoryStream = new MemoryStream();
        var dumpOp = await inputStream.DumpTo(memoryStream);
        memoryStream.Position = 0;
        (await memoryStream.ReadBytes()).Should().BeEquivalentTo(buffer);
    }
}

public static class Ext
{
    public static IObservable<Unit> DumpTo(this IObservable<byte> source, Stream output, TimeSpan? chunkReadTimeout = default, IScheduler scheduler = default, int bufferSize = 4096)
    {
        scheduler ??= Scheduler.Default;
        chunkReadTimeout ??= TimeSpan.FromDays(1);

        return source
            .Buffer(bufferSize)
            .Select(chunk => Observable.FromAsync(ct => output.WriteAsync(chunk.ToArray(), 0, chunk.Count, ct), scheduler).Timeout(chunkReadTimeout.Value, scheduler))
            .Concat();
    }
}