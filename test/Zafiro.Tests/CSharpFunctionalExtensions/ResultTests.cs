using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;
using Zafiro.CSharpFunctionalExtensions;
using Observable = System.Reactive.Linq.Observable;

namespace Zafiro.Tests.CSharpFunctionalExtensions;

public class ResultTests
{
    [Fact]
    public void SelectMany()
    {
        var p = Observable.Return(Result.Success(1));
        var q = Observable.Return(Result.Success("a"));

        var r = from a in p from b in q select a+b;

        var notifications = new Testing.NotificationBuilder<Result<string>>()
            .OnNext("1a", 0)
            .OnCompleted(0)
            .Build();

        var subject = new TestScheduler().CreateObserver<Result<string>>();
        r.Subscribe(subject);

        subject.Messages.Should().BeEquivalentTo(notifications);
    }
}