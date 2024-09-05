using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Progress;
using Zafiro.Reactive;

namespace Zafiro.UI.Jobs.Execution;

public abstract class ExecutionFactory
{
    public static IExecution From<T>(IObservable<T> onExecute, IObservable<IProgress> progress, Maybe<IObservable<bool>> canStart)
    {
        return new StoppableExecution(onExecute.ToSignal(), progress, canStart);
    }

    public static IExecution From(Func<CancellationToken, Task> taskFactory, IObservable<IProgress> progress, Maybe<IObservable<bool>> canStart)
    {
        return new StoppableExecution(Observable.FromAsync(taskFactory).ToSignal(), progress, canStart);
    }

    public static IExecution From<T>(Func<CancellationToken, Task<T>> taskFactory, IObservable<IProgress> progress, Maybe<IObservable<bool>> canStart)
    {
        return new StoppableExecution(Observable.FromAsync(taskFactory).ToSignal(), progress, canStart);
    }

    public static IExecution From(Func<Task> taskFactory, IObservable<IProgress> progress)
    {
        return new UnstoppableExecution(Observable.FromAsync(taskFactory).ToSignal(), progress);
    }

    public static IExecution From(ReactiveCommandBase<Unit, Unit> start, ReactiveCommandBase<Unit, Unit> stop, IObservable<IProgress> progress)
    {
        return new StartStopExecution(start, stop, progress);
    }
}