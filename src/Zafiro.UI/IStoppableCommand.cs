using System.Reactive;
using System.Windows.Input;
using ReactiveUI;

namespace Zafiro.UI;

public interface IStoppableCommand
{
    IObservable<bool> IsExecuting { get; }
    IObservable<bool> CanExecute { get; }
    public ICommand Start { get; }
    public ICommand Stop { get; }
}

public interface IStoppableCommand<TIn, TOut> : IStoppableCommand
{
    public ReactiveCommand<TIn, TOut> StartReactive { get; }
    public ReactiveCommandBase<Unit, Unit> StopReactive { get; }
}