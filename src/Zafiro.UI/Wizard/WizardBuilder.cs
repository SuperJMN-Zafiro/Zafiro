// **1. El contenedor estático que arranca el builder**

using System.Reactive;
using Zafiro.Mixins;
using Zafiro.UI.Wizard;

public static class WizardBuilder
{
    public static SlimWizardBuilder<TPage, TResult> StartWith<TPage, TResult>(
        Func<TPage> pageFactory,
        Func<TPage, TResult> resultFactory,
        Func<TPage, IObservable<bool>> canGoNext)
    {
        return new SlimWizardBuilder<TPage, TResult>(pageFactory, resultFactory, canGoNext);
    }
}

// **2. El builder genérico, sin sufijos “Async” y usando solo Func<>**
public class SlimWizardBuilder<TPage, TResult>
{
    private readonly List<WizardStep> steps;

    // constructor inicial: mete el primer paso
    internal SlimWizardBuilder(
        Func<TPage> pageFactory,
        Func<TPage, TResult> resultFactory,
        Func<TPage, IObservable<bool>> canGoNext)
    {
        steps = new List<WizardStep>
        {
            // la primera factoría no tiene input previo, así que ignoramos el object
            new WizardStep(
                _ => pageFactory(), 
                page => resultFactory((TPage)page),
                page => canGoNext((TPage)page)
            )
        };
    }

    // constructor para clonar la lista compartida
    private SlimWizardBuilder(List<WizardStep> existingSteps)
    {
        steps = existingSteps;
    }

    // **3. AddStep: encadenas otro paso a partir del resultado anterior**
    public SlimWizardBuilder<TNextPage, TNextResult> AddStep<TNextPage, TNextResult>(
        Func<TResult, TNextPage> pageFactory,
        Func<TNextPage, TNextResult> resultFactory, 
        Func<TNextPage, IObservable<bool>> canGoNext)
    {
        steps.Add(new WizardStep(
            prevResult => pageFactory((TResult)prevResult),
            page   => resultFactory((TNextPage)page),
            page => canGoNext((TNextPage)page)
        ));

        return new SlimWizardBuilder<TNextPage, TNextResult>(steps);
    }
}