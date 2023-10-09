using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using Microsoft.Reactive.Testing;
using Xunit;
using Zafiro.CSharpFunctionalExtensions;

namespace Zafiro.Tests.CSharpFunctionalExtensions;

public class ResultTests
{
    [Fact]
    public void SelectMany()
    {
        var p = Observable.Return(Result.Success(1));
        var q = Observable.Return(Result.Success("a"));

        var r = from a in p from b in q select a + b;

        var notifications = new Testing.NotificationBuilder<Result<string>>()
            .OnNext("1a", 0)
            .OnCompleted(0)
            .Build();

        var subject = new TestScheduler().CreateObserver<Result<string>>();
        r.Subscribe(subject);

        subject.Messages.Should().BeEquivalentTo(notifications);
    }

    [Fact]
    public async Task Result_and_Maybe_combine_tasks()
    {
        var a = Task.FromResult(Result.Success(Maybe.From(1)));
        var c = await a.Bind(x => Task.FromResult(Result.Success(Maybe.From(x + 1))));

        c.Should().BeSuccess().And.Subject.Value.Value.Should().Be(2);
    }
}