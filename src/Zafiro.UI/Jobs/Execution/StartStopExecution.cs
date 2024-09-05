using System.Reactive;
using ReactiveUI;
using Zafiro.Progress;

namespace Zafiro.UI.Jobs.Execution;

public class StartStopExecution(ReactiveCommandBase<Unit, Unit> start, ReactiveCommandBase<Unit, Unit> stop, IObservable<IProgress> progress) : IExecution
{
    public ReactiveCommandBase<Unit, Unit> Start { get; } = start;
    public ReactiveCommandBase<Unit, Unit> Stop { get; } = stop;
    public IObservable<IProgress> Progress { get; } = progress;
}