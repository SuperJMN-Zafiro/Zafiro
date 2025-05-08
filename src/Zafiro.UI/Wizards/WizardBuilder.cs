using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards;

public static class WizardBuilder
{
    public static WizardBuilder<TPage, TResult> StartWith<TPage, TResult>(
        Func<TPage> pageFactory,
        Func<TPage, IEnhancedCommand<Result<TResult>>>? nextCommand = null,
        string nextText = "Next")
    {
        var step = new WizardStep<TPage, TResult>(
            _ => pageFactory(),
            nextCommand,
            nextText);

        return new WizardBuilder<TPage, TResult>(new[] { step });
    }
}

public class WizardBuilder<TPage, TResult>
{
    private readonly IReadOnlyList<IWizardStep> steps;

    public WizardBuilder(IEnumerable<IWizardStep> steps)
    {
        this.steps = steps.ToList();
    }

    public WizardBuilder<TNextPage, TNextResult> Then<TNextPage, TNextResult>(
        Func<TResult, TNextPage> pageFactory,
        Func<TNextPage, IEnhancedCommand<Result<TNextResult>>>? nextCommand = null,
        string nextText = "Next")
    {
        var step = new WizardStep<TNextPage, TNextResult>(
            prev => pageFactory((TResult)prev!),
            nextCommand,
            nextText);

        var newSteps = steps.Concat(new[] { step });
        return new WizardBuilder<TNextPage, TNextResult>(newSteps);
    }

    public IReadOnlyList<IWizardStep> BuildSteps()
    {
        return steps;
    }
}