namespace Zafiro.UI.Wizards.Classic.Builder;

/// <summary>
/// Entry points to start composing a Classic wizard.
/// </summary>
public static class WizardBuilder
{
    /// <summary>
    /// Starts a Classic wizard with the provided first step factory.
    /// </summary>
    public static WizardBuilder<TFirst> StartWith<TFirst>(Func<TFirst> start) where TFirst : IStep
    {
        return new WizardBuilder<TFirst>(start);
    }
}

/// <summary>
/// Fluent builder for Classic wizard steps.
/// </summary>
public class WizardBuilder<TCurrent> where TCurrent : IStep
{
    private readonly List<Func<IStep?, IStep>> steps;

    internal WizardBuilder(Func<TCurrent> start)
    {
        steps = new List<Func<IStep?, IStep>> { _ => start() };
    }

    private WizardBuilder(List<Func<IStep?, IStep>> steps)
    {
        this.steps = steps;
    }

    /// <summary>
    /// Adds the next step. Optionally executes an action on the current step before creating the next.
    /// </summary>
    public WizardBuilder<TNext> Then<TNext>(Func<TCurrent, TNext> factory, Action<TCurrent>? action = null) where TNext : IStep
    {
        steps.Add(prev =>
        {
            var current = (TCurrent)prev!;
            action?.Invoke(current);
            return factory(current);
        });
        return new WizardBuilder<TNext>(steps);
    }

    /// <summary>
    /// Finishes the wizard with a result computed from the last step.
    /// </summary>
    public IWizard<TResult> FinishWith<TResult>(Func<TCurrent, TResult> resultFactory)
    {
        return new Wizard<TResult>(
            steps,
            last => resultFactory((TCurrent)last)
        );
    }
}
