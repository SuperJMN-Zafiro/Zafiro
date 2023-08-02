using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;
using Zafiro.IO;
using Zafiro.Testing;
using static System.TimeSpan;

namespace Zafiro.Tests;

public class StreamAsObservableTests
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
    public void Timeout_happens_when_no_data_is_pushed()
    {
        // ARRANGE
        var scheduler = new TestScheduler();
        var testStream = new TestStream(new[]
        {
            (new byte[] { 65 }, FromTicks(1)),
            (new byte[] { 66 }, FromTicks(2)),
            (new byte[] { 67 }, FromTicks(3))
        }, scheduler);

        var observable = testStream
            .ToObservable()
            .Timeout(FromTicks(3), scheduler: scheduler);

        var observer = scheduler.CreateObserver<byte>();

        // ACT
        observable.Subscribe(observer);
        scheduler.AdvanceTo(6);

        // ASSERT
        var expectation = new NotificationBuilder<byte>()
            .OnNext(65, 1)
            .OnNext(66, 2)
            .OnError(new TimeoutException(), 6)
            .Build();

        AreEquivalent(observer.Messages, expectation);
    }

    [Fact]
    public void No_timeout_when_data_is_pushed_on_time()
    {
        // ARRANGE
        var scheduler = new TestScheduler();
        var testStream = new TestStream(new[]
        {
            (new byte[] { 65 }, FromTicks(1)),
            (new byte[] { 66 }, FromTicks(2)),
            (new byte[] { 67 }, FromTicks(2))
        }, scheduler);

        var observable = testStream
            .ToObservable()
            .Timeout(FromTicks(3), scheduler: scheduler);

        var observer = scheduler.CreateObserver<byte>();

        // ACT
        observable.Subscribe(observer);
        scheduler.AdvanceTo(6);

        // ASSERT
        var expectation = new NotificationBuilder<byte>()
            .OnNext(65, FromDays(1).Ticks)
            .OnNext(66, FromDays(2).Ticks)
            .OnNext(66, FromDays(4).Ticks)
            .OnCompleted(4)
            .Build();

        AreEquivalent(observer.Messages, expectation);
    }

    private static void AreEquivalent<T>(IEnumerable<Recorded<Notification<T>>> messages, IEnumerable<Recorded<Notification<T>>> expectation)
    {
        messages.Should().BeEquivalentTo(expectation, ctx =>
        {
            return ctx.Using<Recorded<Notification<T>>>(context =>
            {
                if (context.Expectation.Value is { Exception: { } a } && context.Subject.Value is { Exception: { } b })
                {
                    a.Should().BeOfType(b.GetType());
                }
            }).WhenTypeIs<Recorded<Notification<T>>>();
        });
    }
}