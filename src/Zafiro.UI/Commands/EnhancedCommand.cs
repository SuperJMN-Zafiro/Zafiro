using System.Windows.Input;

namespace Zafiro.UI.Commands;

public static class EnhancedCommand
{
    public static EnhancedCommand<T> Enhance<T>(this IEnhancedCommand<T> enhancedCommand, string? text = null, string? name = null, IObservable<bool>? canExecute = null)
    {
        return new EnhancedCommand<T>(ReactiveCommand.CreateFromObservable(enhancedCommand.Execute, canExecute ?? ((IReactiveCommand)enhancedCommand).CanExecute), text ?? enhancedCommand.Text, name ?? enhancedCommand.Name);
    }

    public static IEnhancedCommand<T, Q> Enhance<T, Q>(this ReactiveCommandBase<T, Q> reactiveCommand, string? text = null, string? name = null)
    {
        return new EnhancedCommand<T, Q>(reactiveCommand, text, name);
    }

    public static EnhancedCommand<T> Enhance<T>(this ReactiveCommandBase<Unit, T> reactiveCommand, string? text = null, string? name = null)
    {
        return new EnhancedCommand<T>(reactiveCommand, text, name);
    }

    public static IEnhancedUnitCommand Enhance(this ReactiveCommandBase<Unit, Unit> reactiveCommand, string? text = null, string? name = null)
    {
        return new EnhancedUnitCommand(reactiveCommand, text, name);
    }

    public static EnhancedCommand<global::CSharpFunctionalExtensions.Result<T>> Create<T>(Func<global::CSharpFunctionalExtensions.Result<T>> execute, IObservable<bool>? canExecute = null, string? text = null, string? name = null)
    {
        return ReactiveCommand.Create(execute, canExecute).Enhance(text, name);
    }

    public static EnhancedCommand<global::CSharpFunctionalExtensions.Result<T>> Create<T>(Func<Task<global::CSharpFunctionalExtensions.Result<T>>> task, IObservable<bool>? canExecute = null, string? text = null, string? name = null)
    {
        return ReactiveCommand.CreateFromTask(task, canExecute).Enhance(text, name);
    }
}

public class EnhancedUnitCommand : EnhancedCommand<Unit, Unit>, IEnhancedCommand<Unit>, IEnhancedUnitCommand
{
    public EnhancedUnitCommand(ReactiveCommandBase<Unit, Unit> reactiveCommandBase, string? text = null, string? name = null) : base(reactiveCommandBase)
    {
    }
}

public class EnhancedCommand<T> : EnhancedCommand<Unit, T>, IEnhancedCommand<T>
{
    public EnhancedCommand(ReactiveCommandBase<Unit, T> reactiveCommandBase, string? text = null, string? name = null) : base(reactiveCommandBase, text, name)
    {
    }
}

public class EnhancedCommand<TParam, TResult> : ReactiveObject, IEnhancedCommand<TParam, TResult>
{
    private readonly ICommand command;
    private readonly ReactiveCommandBase<TParam, TResult> reactiveCommand;

    public EnhancedCommand(ReactiveCommandBase<TParam, TResult> reactiveCommandBase, string? text = null, string? name = null)
    {
        command = reactiveCommandBase;
        reactiveCommand = reactiveCommandBase;
        Name = name;
        Text = text;
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

    public string? Name { get; }
    public string? Text { get; }
}