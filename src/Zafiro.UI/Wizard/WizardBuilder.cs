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

public class SlimWizardBuilder<TPage, TResult>
{
    private readonly List<WizardStep> steps;

    internal SlimWizardBuilder(Func<TPage> pageFactory,
        Func<TPage, IEnhancedCommand<Result<TResult>>>? nextCommand,
        string nextText)
    {
        steps = new List<WizardStep>
        {
            CreateWizardStep(_ => pageFactory(), nextCommand, nextText)
        };
    }

    private SlimWizardBuilder(List<WizardStep> existingSteps)
    {
        steps = existingSteps;
    }

    public SlimWizardBuilder<TNextPage, TNextResult> Then<TNextPage, TNextResult>(
        Func<TResult, TNextPage> pageFactory,
        Func<TNextPage, IEnhancedCommand<Result<TNextResult>>>? nextCommand,
        string nextText = "Next")
    {
        steps.Add(CreateWizardStep(pageFactory, nextCommand, nextText));

        return new SlimWizardBuilder<TNextPage, TNextResult>(steps);
    }

    private WizardStep CreateWizardStep<TNextPage, TNextResult>(Func<TResult, TNextPage> pageFactory, Func<TNextPage, IEnhancedCommand<Result<TNextResult>>>? nextCommand, string nextText)
    {
        return new WizardStep(
            o => pageFactory((TResult)o),
            o =>
            {
                TNextPage nextPage = pageFactory((TResult)o);
                IEnhancedCommand<Result<TNextResult>> enhancedCommand = nextCommand(nextPage);
                return new CommandAdapter<Result<TNextResult>, Result<object>>(enhancedCommand, result => (object)result);
            });
    }

    public Wizard<TResult> Build()
    {
        return new Wizard<TResult>(steps);
    }
}