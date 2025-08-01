using System;
using System.Reactive;
using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim.Builder;

public static class WizardBuilder
{
    public static WizardBuilder<TResult> StartWith<TPage, TResult>(
        Func<TPage> pageFactory,
        Func<TPage, IEnhancedCommand<Result<TResult>>>? nextCommand,
        string title)
    {
        Func<TPage, Unit, IEnhancedCommand<Result<TResult>>>? command = nextCommand == null ? null : (page, _) => nextCommand(page);

        var step = new StepDefinition<Unit, TPage, TResult>(
            (_)=> pageFactory(),
            command,
            title);

        return new WizardBuilder<TResult>(new[] { step });
    }

    public static WizardBuilder<TResult> StartWith<TPage, TResult>(
        Func<TPage> pageFactory,
        Func<TPage, Result<TResult>> nextAction,
        Func<TPage, IObservable<bool>>? canExecute,
        string title,
        string? text = null)
    {
        return StartWith(pageFactory, page => EnhancedCommand.Create(() => nextAction(page), canExecute?.Invoke(page), text), title);
    }

    public static StepBuilder<Unit, TPage> StartWith<TPage>(Func<TPage> pageFactory, string title)
    {
        return new StepBuilder<Unit, TPage>(Array.Empty<IStepDefinition>(), _ => pageFactory(), title);
    }
}

public class WizardBuilder<TResult>(IEnumerable<IStepDefinition> steps)
{
    private readonly List<IStepDefinition> steps = steps.ToList();

    public StepBuilder<TResult, TNextPage> Then<TNextPage>(Func<TResult, TNextPage> pageFactory, string title)
    {
        return new StepBuilder<TResult, TNextPage>(this.steps, prev => pageFactory(prev), title);
    }

    public WizardBuilder<TNextResult> Then<TNextPage, TNextResult>(
        Func<TResult, TNextPage> pageFactory,
        Func<TNextPage, IEnhancedCommand<Result<TNextResult>>>? nextCommand,
        string title)
    {
        Func<TNextPage, TResult, IEnhancedCommand<Result<TNextResult>>>? command = nextCommand == null ? null : (page, _) => nextCommand(page);

        var step = new StepDefinition<TResult, TNextPage, TNextResult>(
            prev => pageFactory(prev),
            command,
            title);

        var newSteps = steps.Append(step);
        return new WizardBuilder<TNextResult>(newSteps);
    }

    public WizardBuilder<TNextResult> Then<TNextPage, TNextResult>(
        Func<TResult, TNextPage> pageFactory,
        Func<TNextPage, Result<TNextResult>> nextAction,
        Func<TNextPage, IObservable<bool>>? canExecute,
        string title,
        string? text = null)
    {
        return Then(pageFactory, page => EnhancedCommand.Create(() => nextAction(page), canExecute?.Invoke(page), text), title);
    }

    public WizardBuilder<TNextResult> Then<TNextPage, TNextResult>(
        Func<TResult, TNextPage> pageFactory,
        Func<TNextPage, TResult, IEnhancedCommand<Result<TNextResult>>>? nextCommand,
        string title)
    {
        Func<TNextPage, TResult, IEnhancedCommand<Result<TNextResult>>>? command = nextCommand == null ? null : (page, prev) => nextCommand(page, prev);

        var step = new StepDefinition<TResult, TNextPage, TNextResult>(
            prev => pageFactory(prev),
            command,
            title);

        var newSteps = steps.Append(step);
        return new WizardBuilder<TNextResult>(newSteps);
    }

    public WizardBuilder<TNextResult> Then<TNextPage, TNextResult>(
        Func<TResult, TNextPage> pageFactory,
        Func<TNextPage, TResult, Result<TNextResult>> nextAction,
        Func<TNextPage, TResult, IObservable<bool>>? canExecute,
        string title,
        string? text = null)
    {
        return Then(pageFactory, (page, prev) => EnhancedCommand.Create(() => nextAction(page, prev), canExecute?.Invoke(page, prev), text), title);
    }

    public SlimWizard<TResult> WithCommitFinalStep()
    {
        return BuildWizard(StepKind.Commit);
    }

    public SlimWizard<TResult> WithCompletionFinalStep()
    {
        return BuildWizard(StepKind.Completion);
    }

    private SlimWizard<TResult> BuildWizard(StepKind lastStepKind)
    {
        List<IStepDefinition> normal = steps[..^1];
        IStepDefinition lastStep = steps[^1];
    
        var normalSteps = normal.Select(def => new WizardStep(StepKind.Normal, def.Title, def.CreatePage, def.GetNextCommand));
        var allSteps = normalSteps.Append(new WizardStep(lastStepKind, lastStep.Title, lastStep.CreatePage, lastStep.GetNextCommand));
    
        return new SlimWizard<TResult>(allSteps.Cast<IWizardStep>().ToList());
    }
}