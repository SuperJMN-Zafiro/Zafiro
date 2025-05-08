using System.Windows.Input;

namespace Zafiro.UI.Commands;

public static class EnhancedCommand
{
    public static IEnhancedCommand<T, Q> Enhance<T, Q>(this ReactiveCommandBase<T, Q> reactiveCommand)
    {
        return new EnhancedCommand<T, Q>(reactiveCommand);
    }

    public static EnhancedCommand<T> Enhance<T>(this ReactiveCommandBase<Unit, T> reactiveCommand)
    {
        return new EnhancedCommand<T>(reactiveCommand);
    }

    public static IEnhancedUnitCommand Enhance(this ReactiveCommandBase<Unit, Unit> reactiveCommand)
    {
        return new EnhancedUnitCommand(reactiveCommand);
    }
}

public class EnhancedUnitCommand : EnhancedCommand<Unit, Unit>, IEnhancedCommand<Unit>, IEnhancedUnitCommand
{
    public EnhancedUnitCommand(ReactiveCommandBase<Unit, Unit> reactiveCommandBase) : base(reactiveCommandBase)
    {
    }
}

public class EnhancedCommand<T> : EnhancedCommand<Unit, T>, IEnhancedCommand<T>
{
    public EnhancedCommand(ReactiveCommandBase<Unit, T> reactiveCommandBase) : base(reactiveCommandBase)
    {
    }
}

public class EnhancedCommand<TParam, TResult> : IEnhancedCommand<TParam, TResult>
{
    private readonly ICommand command;
    private readonly ReactiveCommandBase<TParam, TResult> reactiveCommand;

    public EnhancedCommand(ReactiveCommandBase<TParam, TResult> reactiveCommandBase)
    {
        command = reactiveCommandBase;
        reactiveCommand = reactiveCommandBase;
    }

    bool ICommand.CanExecute(object? parameter) => command.CanExecute(parameter);

    public void Execute(object? parameter) => command.Execute(parameter);

    public event EventHandler? CanExecuteChanged
    {
        add => command.CanExecuteChanged += value;
        remove => command.CanExecuteChanged -= value;
    }

    public IObservable<Exception> ThrownExceptions => reactiveCommand.ThrownExceptions;

    public IObservable<bool> IsExecuting => reactiveCommand.IsExecuting;

    public IObservable<bool> CanExecute => ((IReactiveCommand)command).CanExecute;

    public IDisposable Subscribe(IObserver<TResult> observer) => reactiveCommand.Subscribe(observer);

    public IObservable<TResult> Execute(TParam parameter) => reactiveCommand.Execute(parameter);

    public IObservable<TResult> Execute() => reactiveCommand.Execute();

    public void Dispose()
    {
        reactiveCommand.Dispose();
    }
}