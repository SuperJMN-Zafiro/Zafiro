using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizard;

public record WizardStep(
    Func<object?, object> PageFactory,
    Func<object?, IEnhancedCommand<Result<object>>>? NextCommand,
    string? NextText = "Next"
);