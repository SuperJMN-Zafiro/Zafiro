using System.Reactive.Linq;
using System.Windows.Input;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards;

public class CommandAdapter<TSource, TTarget> : ReactiveObject, IEnhancedCommand<TTarget>
{
    private readonly Func<TSource, TTarget> converter;
    private readonly IEnhancedCommand<TSource> originalCommand;

    public CommandAdapter(IEnhancedCommand<TSource> originalCommand, Func<TSource, TTarget> converter, string? name = "")
    {
        this.originalCommand = originalCommand;
        this.converter = converter;
        Name = originalCommand.Name == "" ? name : originalCommand.Name;
    }

    public void Dispose()
    {
        originalCommand.Dispose();
    }

    public IObservable<Exception> ThrownExceptions => originalCommand.ThrownExceptions;

    public IObservable<bool> IsExecuting => originalCommand.IsExecuting;

    public IObservable<bool> CanExecute => ((IReactiveCommand)originalCommand).CanExecute;

    bool ICommand.CanExecute(object? parameter)
    {
        return originalCommand.CanExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        originalCommand.Execute(parameter);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => originalCommand.CanExecuteChanged += value;
        remove => originalCommand.CanExecuteChanged -= value;
    }

    public IDisposable Subscribe(IObserver<TTarget> observer)
    {
        return originalCommand.Select(source => converter(source)).Subscribe(observer);
    }

    public IObservable<TTarget> Execute(Unit parameter)
    {
        return originalCommand.Execute().Select(converter);
    }

    public IObservable<TTarget> Execute()
    {
        return originalCommand.Execute().Select(converter);
    }

    public string? Name { get; }
    public string? Text => originalCommand.Text;
}