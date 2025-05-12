using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim;

public static class WizardBuilder
{
    public static WizardBuilder<TResult> StartWith<TPage, TResult>(
        Func<TPage> pageFactory,
        Func<TPage, IEnhancedCommand<Result<TResult>>>? nextCommand,
        string title)
    {
        var step = new WizardStep<TPage, TResult>(
            _ => pageFactory(),
            nextCommand,
            title);

        return new WizardBuilder<TResult>(new[] { step });
    }
}

public class WizardBuilder<TResult>
{
    private readonly IReadOnlyList<IWizardStep> steps;

    public WizardBuilder(IEnumerable<IWizardStep> steps)
    {
        this.steps = steps.ToList();
    }

    public WizardBuilder<TNextResult> Then<TNextPage, TNextResult>(
        Func<TResult, TNextPage> pageFactory,
        Func<TNextPage, IEnhancedCommand<Result<TNextResult>>>? nextCommand,
        string title)
    {
        var step = new WizardStep<TNextPage, TNextResult>(
            prev => pageFactory((TResult)prev!),
            nextCommand,
            title);

        var newSteps = steps.Append(step);
        return new WizardBuilder<TNextResult>(newSteps);
    }

    public SlimWizard<TResult> Build()
    {
        return new SlimWizard<TResult>(steps.ToList());
    }
}