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
        Func<TPage, object?, IEnhancedCommand<Result<TResult>>>? command = nextCommand == null ? null : (page, _) => nextCommand(page);

        var step = new StepDefinition<TPage, TResult>(
            _ => pageFactory(),
            command,
            title);

        return new WizardBuilder<TResult>(new[] { step });
    }
}

public class WizardBuilder<TResult>(IEnumerable<IStepDefinition> steps)
{
    private readonly List<IStepDefinition> steps = steps.ToList();

    public WizardBuilder<TNextResult> Then<TNextPage, TNextResult>(
        Func<TResult, TNextPage> pageFactory,
        Func<TNextPage, IEnhancedCommand<Result<TNextResult>>>? nextCommand,
        string title)
    {
        Func<TNextPage, object?, IEnhancedCommand<Result<TNextResult>>>? command = nextCommand == null ? null : (page, _) => nextCommand(page);

        var step = new StepDefinition<TNextPage, TNextResult>(
            prev => pageFactory((TResult)prev!),
            command,
            title);

        var newSteps = steps.Append(step);
        return new WizardBuilder<TNextResult>(newSteps);
    }

    public WizardBuilder<TNextResult> Then<TNextPage, TNextResult>(
        Func<TResult, TNextPage> pageFactory,
        Func<TNextPage, TResult, IEnhancedCommand<Result<TNextResult>>>? nextCommand,
        string title)
    {
        Func<TNextPage, object?, IEnhancedCommand<Result<TNextResult>>>? command = nextCommand == null ? null : (page, prev) => nextCommand(page, (TResult)prev!);

        var step = new StepDefinition<TNextPage, TNextResult>(
            prev => pageFactory((TResult)prev!),
            command,
            title);

        var newSteps = steps.Append(step);
        return new WizardBuilder<TNextResult>(newSteps);
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