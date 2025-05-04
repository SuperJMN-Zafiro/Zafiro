using CSharpFunctionalExtensions;

namespace Zafiro.UI.Wizard;

public record WizardStep(
    Func<object?, object> PageFactory,
    Func<object, Result<object>> OnNext,
    Func<object, IObservable<bool>> CanGoNext,
    string? NextText = "Next"
);