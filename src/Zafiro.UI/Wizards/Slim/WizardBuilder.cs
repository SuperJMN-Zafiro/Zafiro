using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim;

public static class WizardBuilder
{
    public static WizardBuilder<TPage, TResult> StartWith<TPage, TResult>(
        Func<TPage> pageFactory,
        Func<TPage, IEnhancedCommand<Result<TResult>>>? nextCommand,
        string title,
        string nextText = "Next")
    {
        var step = new WizardStep<TPage, TResult>(
            _ => pageFactory(),
            nextCommand,
            title,
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
        Func<TNextPage, IEnhancedCommand<Result<TNextResult>>>? nextCommand,
        string title,
        string nextText = "Next")
    {
        var step = new WizardStep<TNextPage, TNextResult>(
            prev => pageFactory((TResult)prev!),
            nextCommand,
            title,
            nextText);

        var newSteps = steps.Append(step);
        return new WizardBuilder<TNextPage, TNextResult>(newSteps);
    }

    public SlimWizard<TResult> Build()
    {
        return new SlimWizard<TResult>(steps.ToList());
    }

    public IReadOnlyList<IWizardStep> BuildSteps()
    {
        return steps;
    }
}