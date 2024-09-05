using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Progress;

namespace Zafiro.UI.Jobs.Execution;

public class StoppableExecution : IExecution
{
    public StoppableExecution(IObservable<Unit> observable, IObservable<IProgress> progress, Maybe<IObservable<bool>> canStart)
    {
        Progress = progress;
        var stoppable = StoppableCommand.Create(observable, canStart);
        Start = stoppable.StartReactive;
        Stop = stoppable.StopReactive;
    }

    public ReactiveCommandBase<Unit, Unit> Start { get; }
    public ReactiveCommandBase<Unit, Unit> Stop { get; }
    public IObservable<IProgress> Progress { get; }
}