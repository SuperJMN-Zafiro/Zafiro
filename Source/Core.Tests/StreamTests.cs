using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Xunit;
using Xunit.Sdk;
using Zafiro.Core.Mixins;

namespace Core.Tests;

public class StreamTests
{
    [Fact]
    public async Task Contents_are_equivalent()
    {
        var bytes = "Pepito"u8.ToArray();
        var sut = new MemoryStream(bytes).ToObservable();
        var allBytes = await sut.ToList();

        allBytes.Should().BeEquivalentTo(bytes);
    }

    [Fact]
    public async Task Timeout_happens()
    {
        var testScheduler = new TestScheduler();
        var testStream = new TestStream(new[]
        {
            ("Pepito"u8.ToArray(), TimeSpan.FromSeconds(1)),
            ("Pepito"u8.ToArray(), TimeSpan.FromSeconds(1)),
            ("Pepito"u8.ToArray(), TimeSpan.FromSeconds(1)),
            ("Pepito"u8.ToArray(), TimeSpan.FromSeconds(0.5)),
        }, testScheduler);

        testScheduler.Start();
        var task = async () => await testStream.ToObservable().Timeout(TimeSpan.FromSeconds(2));
        await task.Should().ThrowAsync<TimeoutException>();
    }

    [Fact]
    public async Task Timeout_does_not_happen()
    {
        var testScheduler = new TestScheduler();
        var testStream = new TestStream(new[]
        {
            (new byte[]{ 65 }, TimeSpan.FromSeconds(1)),
            (new byte[]{ 66 }, TimeSpan.FromSeconds(2)),
            (new byte[]{ 67 }, TimeSpan.FromSeconds(3)),
        }, testScheduler);

        var sut = testStream.ToObservable().Timeout(TimeSpan.FromSeconds(2.5), scheduler: testScheduler);
        var testableObserver = testScheduler.CreateObserver<byte>();
        sut.Subscribe(testableObserver);

        testScheduler.AdvanceTo(TimeSpan.FromSeconds(6).Ticks);
        var messages = testableObserver.Messages;

        messages.Should().BeEquivalentTo(new[]
        {
            new Recorded<Notification<byte>>(10000000, Notification.CreateOnNext<byte>(65)),
            new Recorded<Notification<byte>>(30000000, Notification.CreateOnNext<byte>(66)),
            new Recorded<Notification<byte>>(55000000, Notification.CreateOnError<byte>(new TimeoutException())),
        }, ctx => ctx.Using<Recorded<Notification<byte>>>(context =>
        {
            if (context.Expectation.Value is { Exception: { } a } && context.Subject.Value is { Exception: { } b })
            {
                a.Should().BeOfType(b.GetType());
            }
        }).WhenTypeIs<Recorded<Notification<byte>>>());
    }

    [Fact]
    public async Task Timeout_does_not_happen_default_scheduler()
    {
        var testStream = new TestStream(new[]
        {
            ("a"u8.ToArray(), TimeSpan.FromSeconds(4)),
            ("b"u8.ToArray(), TimeSpan.FromSeconds(5)),
        }, Scheduler.Default);

        await testStream.ToObservable().Timeout(TimeSpan.FromSeconds(6)).ToList();
    }

    [Fact]
    public async Task Timeout_happens_default_scheduler()
    {
        var testStream = new TestStream(new[]
        {
            ("a"u8.ToArray(), TimeSpan.FromSeconds(4)),
            ("b"u8.ToArray(), TimeSpan.FromSeconds(6)),
        }, Scheduler.Default);

        var task = async () => await testStream.ToObservable().Timeout(TimeSpan.FromSeconds(5)).ToList();
        await task.Should().ThrowAsync<TimeoutException>();
    }
}

public class TestStream : Stream
{
    private readonly IScheduler scheduler;
    private readonly (byte[], TimeSpan)[] datas;
    private int read = 0; // Start with -1 to indicate that no data has been read yet

    public TestStream((byte[], TimeSpan)[] datas, IScheduler scheduler)
    {
        this.scheduler = scheduler;
        this.datas = datas;
        this.read = 0; // Start reading from the first data array
    }

    public override void Flush()
    {
        throw new System.NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (read == datas.Length)
        {
            return 0;
        }

        datas[read].Item1.CopyTo(buffer, offset);
        scheduler.Sleep(datas[read].Item2);
        var readBytes = datas[read].Item1.Length;
        read++;
        return readBytes;
    }


    // Other methods remain unchanged...
    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (read == datas.Length)
        {
            return 0;
        }

        datas[read].Item1.CopyTo(buffer, offset);
        await scheduler.Sleep(datas[read].Item2);
        var readBytes = datas[read].Item1.Length;
        read++;
        return readBytes;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length { get; }
    public override long Position { get; set; }
}
