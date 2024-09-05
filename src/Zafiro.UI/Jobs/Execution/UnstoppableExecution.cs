using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Progress;
using Zafiro.Reactive;

namespace Zafiro.UI.Jobs.Execution;

public class UnstoppableExecution : IExecution
{
    public UnstoppableExecution(IObservable<Unit> observable, IObservable<IProgress> progress)
    {
        Progress = progress;
        var stoppable = StoppableCommand.Create(observable.ToSignal, Maybe<IObservable<bool>>.None);
        Start = stoppable.StartReactive;
    }

    public ReactiveCommandBase<Unit, Unit> Start { get; }
    public ReactiveCommandBase<Unit, Unit>? Stop { get; } = null;
    public IObservable<IProgress> Progress { get; }
}