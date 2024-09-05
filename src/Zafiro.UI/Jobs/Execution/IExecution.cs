using System.Reactive;
using ReactiveUI;
using Zafiro.Progress;

namespace Zafiro.UI.Jobs.Execution;

public interface IExecution
{
    public ReactiveCommandBase<Unit, Unit> Start { get; }
    public ReactiveCommandBase<Unit, Unit>? Stop { get; }
    public IObservable<IProgress> Progress { get; }
}