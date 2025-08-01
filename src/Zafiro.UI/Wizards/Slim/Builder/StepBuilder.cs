using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim.Builder;

public class StepBuilder<TPrevious, TPage>(IEnumerable<IStepDefinition> previousSteps, Func<TPrevious, TPage> pageFactory, string title)
{
    public WizardBuilder<TResult> ProceedWith<TResult>(Func<TPage, IEnhancedCommand<Result<TResult>>> nextCommand)
    {
        var step = new StepDefinition<TPrevious, TPage, TResult>(pageFactory, (page, _) => nextCommand(page), title);
        var steps = previousSteps.Append(step);
        return new WizardBuilder<TResult>(steps);
    }

    public WizardBuilder<TResult> ProceedWith<TResult>(Func<TPage, TPrevious, IEnhancedCommand<Result<TResult>>> nextCommand)
    {
        var step = new StepDefinition<TPrevious, TPage, TResult>(pageFactory, nextCommand, title);
        var steps = previousSteps.Append(step);
        return new WizardBuilder<TResult>(steps);
    }
}