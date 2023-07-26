using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Collections;
using Microsoft.Reactive.Testing;
using Xunit;
using Zafiro.Core.Mixins;
using static System.TimeSpan;

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
            ("Pepito"u8.ToArray(), FromSeconds(1)),
            ("Pepito"u8.ToArray(), FromSeconds(1)),
            ("Pepito"u8.ToArray(), FromSeconds(1)),
            ("Pepito"u8.ToArray(), FromSeconds(0.5))
        }, testScheduler);

        testScheduler.Start();
        var task = async () => await testStream.ToObservable().Timeout(FromSeconds(2));
        await task.Should().ThrowAsync<TimeoutException>();
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
            .OnNext(1, 65)
            .OnNext(2, 66)
            .OnError(6, new TimeoutException())
            .Build();

        AreEquivalent(observer.Messages, expectation);
    }

    [Fact]
    public async Task Timeout_does_not_happen_default_scheduler()
    {
        var testStream = new TestStream(new[]
        {
            ("a"u8.ToArray(), FromSeconds(4)),
            ("b"u8.ToArray(), FromSeconds(5))
        }, Scheduler.Default);

        await testStream.ToObservable().Timeout(FromSeconds(6)).ToList();
    }

    [Fact]
    public async Task Timeout_happens_default_scheduler()
    {
        var testStream = new TestStream(new[]
        {
            ("a"u8.ToArray(), FromSeconds(4)),
            ("b"u8.ToArray(), FromSeconds(6))
        }, Scheduler.Default);

        var task = async () => await testStream.ToObservable().Timeout(FromSeconds(5)).ToList();
        await task.Should().ThrowAsync<TimeoutException>();
    }

    private static AndConstraint<GenericCollectionAssertions<Recorded<Notification<T>>>> AreEquivalent<T>(IEnumerable<Recorded<Notification<T>>> messages, IEnumerable<Recorded<Notification<T>>> expectation)
    {
        return messages.Should().BeEquivalentTo(expectation, ctx =>
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

public class Notify
{
    public static Recorded<Notification<T>> OnNext<T>(int time, T value)
    {
        return new Recorded<Notification<T>>(time, Notification.CreateOnNext(value));
    }

    public static Recorded<Notification<T>> OnCompleted<T>(int time)
    {
        return new Recorded<Notification<T>>(time, Notification.CreateOnCompleted<T>());
    }

    public static Recorded<Notification<T>> OnError<T>(int time, Exception exception)
    {
        return new Recorded<Notification<T>>(time, Notification.CreateOnError<T>(exception));
    }
}

public class NotificationBuilder<T>
{
    private readonly List<Recorded<Notification<T>>> notifications = new();

    public NotificationBuilder<T> OnNext(int time, T value)
    {
        notifications.Add(new Recorded<Notification<T>>(time, Notification.CreateOnNext(value)));
        return this;
    }

    public NotificationBuilder<T> OnCompleted(int time)
    {
        notifications.Add(new Recorded<Notification<T>>(time, Notification.CreateOnCompleted<T>()));
        return this;
    }

    public NotificationBuilder<T> OnError(int time, Exception exception)
    {
        notifications.Add(new Recorded<Notification<T>>(time, Notification.CreateOnError<T>(exception)));
        return this;
    }

    public IEnumerable<Recorded<Notification<T>>> Build()
    {
        return notifications.AsReadOnly();
    }
}