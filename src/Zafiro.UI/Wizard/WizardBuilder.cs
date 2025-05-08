// **1. El contenedor estático que arranca el builder**

using System.Reactive.Linq;
using System.Windows.Input;
using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizard;

public static class WizardBuilder
{
    public static SlimWizardBuilder<TPage, TResult> StartWith<TPage, TResult>(
        Func<TPage> pageFactory,
        Func<TPage, IEnhancedCommand<Result<TResult>>>? nextCommand,
        string? nextText = "Next")
    {
        return new SlimWizardBuilder<TPage, TResult>(pageFactory, nextCommand, nextText);
    }
}

// **2. El builder genérico, sin sufijos “Async” y usando solo Func<>**
public class SlimWizardBuilder<TPage, TResult>
{
    private readonly List<WizardStep> steps;

    internal SlimWizardBuilder(Func<TPage> pageFactory,
        Func<TPage, IEnhancedCommand<Result<TResult>>>? nextCommand,
        string nextText)
    {
        steps = new List<WizardStep>
        {
            CreateWizardStep<TPage, TResult>(_ => pageFactory(), nextCommand, nextText)
        };
    }

    // constructor para clonar la lista compartida
    private SlimWizardBuilder(List<WizardStep> existingSteps)
    {
        steps = existingSteps;
    }

    // **3. Then: encadenas otro paso a partir del resultado anterior**
    public SlimWizardBuilder<TNextPage, TNextResult> Then<TNextPage, TNextResult>(
        Func<TResult, TNextPage> pageFactory,
        Func<TNextPage, IEnhancedCommand<Result<TNextResult>>>? nextCommand,
        string nextText = "Next")
    {
        steps.Add(CreateWizardStep(pageFactory, nextCommand, nextText));

        return new SlimWizardBuilder<TNextPage, TNextResult>(steps);
    }

    private static WizardStep CreateWizardStep<TNextPage, TNextResult>(Func<TResult, TNextPage> pageFactory, Func<TPage, IEnhancedCommand<Result<TResult>>>? nextCommand, string nextText)
    {
        return new WizardStep(
            prevResult => pageFactory((TResult)prevResult),
            NextCommand: prevResult =>
            {
                throw new NotImplementedException();
                // var nextPage = pageFactory((TResult)prevResult);
                // return new CommandAdapter<Result<TNextResult>, Result<object>>(nextCommand(nextPage), result => (object)result);
            })
        {
            NextText = nextText
        };
    }

    public Wizard<TResult> Build()
    {
        return new Wizard<TResult>(steps);
    }
}

public class CommandAdapter<TSource, TTarget> : IEnhancedCommand<TTarget>
{
    private readonly Func<TSource, TTarget> converter;
    private readonly IEnhancedCommand<TSource> originalCommand;

    public CommandAdapter(IEnhancedCommand<TSource> originalCommand, Func<TSource, TTarget> converter)
    {
        this.originalCommand = originalCommand;
        this.converter = converter;
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
        return Execute().Subscribe(observer);
    }

    public IObservable<TTarget> Execute(Unit parameter)
    {
        return originalCommand.Execute().Select(converter);
    }

    public IObservable<TTarget> Execute()
    {
        return originalCommand.Execute().Select(converter);
    }
}