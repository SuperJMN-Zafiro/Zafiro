using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim.Builder;

public class StepDefinition<TPrevious, TPage, TResult>(
    Func<TPrevious, TPage> pageFactory,
    Func<TPage, TPrevious, IEnhancedCommand<Result<TResult>>>? nextCommandFactory,
    string title)
    : IStepDefinition
{
    private TPrevious previousResult = default!;

    public string Title { get; } = title;

    public object CreatePage(object? previousResult)
    {
        this.previousResult = previousResult is null ? default! : (TPrevious)previousResult;
        return pageFactory(this.previousResult);
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