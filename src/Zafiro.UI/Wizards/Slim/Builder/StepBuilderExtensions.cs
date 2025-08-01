using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;
using Zafiro.UI;

namespace Zafiro.UI.Wizards.Slim.Builder;

public static class StepBuilderExtensions
{
    public static WizardBuilder<TResult> NextWhenValid<TPage, TResult>(this StepBuilder<TPage> builder, string? text, Func<TPage, Result<TResult>> nextAction) where TPage : IValidatable
    {
        return builder.NextWith(page => EnhancedCommand.Create(() => nextAction(page), page.IsValid, text));
    }
}

