using System.Windows.Input;
using CSharpFunctionalExtensions;

namespace Zafiro.UI.Commands;

public static class EnhancedCommand
{
    public static IEnhancedCommand<Result<T>> Create<T>(Func<Result<T>> execute, IObservable<bool>? canExecute = null, string? text = null, string? name = null)
    {
        return ReactiveCommand.Create(execute, canExecute).Enhance(text, name);
    }

    public static IEnhancedCommand<Result<T>> Create<T>(Func<Task<Result<T>>> task, IObservable<bool>? canExecute = null, string? text = null, string? name = null)
    {
        return ReactiveCommand.CreateFromTask(task, canExecute).Enhance(text, name);
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

    public new IObservable<Exception> ThrownExceptions => reactiveCommand.ThrownExceptions;

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