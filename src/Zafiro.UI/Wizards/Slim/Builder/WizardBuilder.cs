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
        var step = new WizardStepDef<TPage, TResult>(
            _ => pageFactory(),
            nextCommand,
            title);

        return new WizardBuilder<TResult>(new[] { step });
    }
}

public class WizardBuilder<TResult>
{
    private readonly List<IWizardStepDef> steps;

    public WizardBuilder(IEnumerable<IWizardStepDef> steps)
    {
        this.steps = steps.ToList();
    }

    public WizardBuilder<TNextResult> Then<TNextPage, TNextResult>(
        Func<TResult, TNextPage> pageFactory,
        Func<TNextPage, IEnhancedCommand<Result<TNextResult>>>? nextCommand,
        string title)
    {
        var step = new WizardStepDef<TNextPage, TNextResult>(
            prev => pageFactory((TResult)prev!),
            nextCommand,
            title);

        var newSteps = steps.Append(step);
        return new WizardBuilder<TNextResult>(newSteps);
    }

    public SlimWizard<TResult> Commit()
    {
        List<IWizardStepDef> normal = steps[..^1];
        IWizardStepDef lastStep = steps[^1];
        
        var normalSteps = normal.Select(IWizardStep (def) => new CustomWizardStep(StepKind.Normal, def.Title, def.CreatePage, def.GetNextCommand));
        var allSteps = normalSteps.Append(new CustomWizardStep(StepKind.Commit, lastStep.Title, lastStep.CreatePage, lastStep.GetNextCommand));
        return new SlimWizard<TResult>(allSteps.ToList());
    }
    
    public SlimWizard<TResult> Completion()
    {
        List<IWizardStepDef> normal = steps[..^1];
        IWizardStepDef lastStep = steps[^1];
        
        var normalSteps = normal.Select(IWizardStep (def) => new CustomWizardStep(StepKind.Normal, def.Title, def.CreatePage, def.GetNextCommand));
        var allSteps = normalSteps.Append(new CustomWizardStep(StepKind.Completion, lastStep.Title, lastStep.CreatePage, lastStep.GetNextCommand));
        return new SlimWizard<TResult>(allSteps.ToList());
    }
}