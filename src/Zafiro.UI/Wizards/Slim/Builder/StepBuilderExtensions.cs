using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim.Builder;

public static class StepBuilderExtensions
{
    public static WizardBuilder<TResult> ProceedWithResultWhenValid<TPrevious, TPage, TResult>(this StepBuilder<TPrevious, TPage> builder, Func<TPage, Result<TResult>> nextAction, string? text = "Next") where TPage : IValidatable
    {
        return builder.ProceedWith(page => EnhancedCommand.Create(() => nextAction(page), page.IsValid, text));
    }

    public static WizardBuilder<TResult> ProceedWithResultWhenValid<TPrevious, TPage, TResult>(this StepBuilder<TPrevious, TPage> builder, Func<TPage, TPrevious, Result<TResult>> nextAction, string? text = "Next") where TPage : IValidatable
    {
        return builder.ProceedWith((page, prev) => EnhancedCommand.Create(() => nextAction(page, prev), page.IsValid, text));
    }
}