using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;
using Zafiro.UI;

namespace Zafiro.UI.Wizards.Slim.Builder;

public static class StepBuilderExtensions
{
    public static WizardBuilder<TResult> ProceedWithResultWhenValid<TPage, TResult>(this StepBuilder<TPage> builder, Func<TPage, Result<TResult>> nextAction, string? text = null) where TPage : IValidatable
    {
        return builder.ProceedWith(page => EnhancedCommand.Create(() => nextAction(page), page.IsValid, text));
    }
}

