// **1. El contenedor estático que arranca el builder**

using CSharpFunctionalExtensions;

namespace Zafiro.UI.Wizard;

public static class WizardBuilder
{
    public static SlimWizardBuilder<TPage, TResult> StartWith<TPage, TResult>(
        Func<TPage> pageFactory,
        Func<TPage, Task<Result<TResult>>> resultFactory,
        Func<TPage, IObservable<bool>> canGoNext,
        string? nextText = "Next")
    {
        return new SlimWizardBuilder<TPage, TResult>(pageFactory, resultFactory, canGoNext, nextText);
    }
}

// **2. El builder genérico, sin sufijos “Async” y usando solo Func<>**
public class SlimWizardBuilder<TPage, TResult>
{
    private readonly List<WizardStep> steps;

    // constructor inicial: mete el primer paso
    internal SlimWizardBuilder(Func<TPage> pageFactory,
        Func<TPage, Task<Result<TResult>>> resultFactory,
        Func<TPage, IObservable<bool>> canGoNext,
        string nextText)
    {
        steps = new List<WizardStep>
        {
            // la primera factoría no tiene input previo, así que ignoramos el object
            new(
                _ => pageFactory(),
                page => resultFactory((TPage)page).Map(result => (object)result),
                page => canGoNext((TPage)page),
                nextText
            )
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
        Func<TNextPage, Task<Result<TNextResult>>> resultFactory,
        Func<TNextPage, IObservable<bool>> canGoNext,
        string nextText = "Next")
    {
        steps.Add(new WizardStep(
            prevResult => pageFactory((TResult)prevResult),
            page => resultFactory((TNextPage)page).Map(result => (object)result),
            page => canGoNext((TNextPage)page)
        )
        {
            NextText = nextText
        });

        return new SlimWizardBuilder<TNextPage, TNextResult>(steps);
    }

    public Wizard<TNextResult> FinishWith<TNextPage, TNextResult>(Func<TResult, TNextPage> pageFactory,
        Func<TNextPage, Task<Result<TNextResult>>> resultFactory,
        Func<TNextPage, IObservable<bool>> canGoNext,
        string? nextText = "Next")
    {
        steps.Add(new WizardStep(
            prevResult => pageFactory((TResult)prevResult),
            page => resultFactory((TNextPage)page).Map(result => (object)result),
            page => canGoNext((TNextPage)page)
        )
        {
            NextText = nextText
        });

        return new Wizard<TNextResult>(steps);
    }
}