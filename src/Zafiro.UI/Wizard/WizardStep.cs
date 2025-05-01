using CSharpFunctionalExtensions;

public class WizardStep
{
    public Func<object?, object> PageFactory { get; }
    public Func<object, Result<object>> OnNext { get; }

    public WizardStep(
        Func<object?, object> pageFactory,
        Func<object, Result<object>> onNext)
    {
        PageFactory = pageFactory;
        OnNext = onNext;
    }
}