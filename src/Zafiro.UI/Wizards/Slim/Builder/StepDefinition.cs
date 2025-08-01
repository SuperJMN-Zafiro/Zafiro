using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim.Builder;

public class StepDefinition<TPage, TResult>(
    Func<object?, TPage> pageFactory,
    Func<TPage, object?, IEnhancedCommand<Result<TResult>>>? nextCommandFactory,
    string title)
    : IStepDefinition
{
    private object? previousResult;

    public string Title { get; } = title;

    public object CreatePage(object? previousResult)
    {
        this.previousResult = previousResult;
        return pageFactory(previousResult);
    }

    public IEnhancedCommand<Result<object>>? GetNextCommand(object page)
    {
        if (nextCommandFactory == null)
            return null;

        var typedPage = (TPage)page;
        var typedCommand = nextCommandFactory(typedPage, previousResult);
        return new CommandAdapter<Result<TResult>, Result<object>>(typedCommand, result => result.Map(x => (object)x));
    }
}