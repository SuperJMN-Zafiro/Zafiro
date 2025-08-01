using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim.Builder;

public class StepBuilder<TPage>(IEnumerable<IStepDefinition> previousSteps, Func<object?, TPage> pageFactory, string title)
{
    private readonly IEnumerable<IStepDefinition> previousSteps = previousSteps;
    private readonly Func<object?, TPage> pageFactory = pageFactory;
    private readonly string title = title;

    public WizardBuilder<TResult> ProceedWith<TResult>(Func<TPage, IEnhancedCommand<Result<TResult>>> nextCommand)
    {
        var step = new StepDefinition<TPage, TResult>(pageFactory, (page, _) => nextCommand(page), title);
        var steps = previousSteps.Append(step);
        return new WizardBuilder<TResult>(steps);
    }
}
