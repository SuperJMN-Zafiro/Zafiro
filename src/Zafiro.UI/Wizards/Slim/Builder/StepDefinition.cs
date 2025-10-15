using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim.Builder;

/// <summary>
/// Default implementation of IStepDefinition for Slim wizards.
/// </summary>
/// <typeparam name="TPrevious">The type of the previous result.</typeparam>
/// <typeparam name="TPage">The page type.</typeparam>
/// <typeparam name="TResult">The type produced by the next step.</typeparam>
public class StepDefinition<TPrevious, TPage, TResult>(
    Func<TPrevious, TPage> pageFactory,
    Func<TPage, TPrevious, IEnhancedCommand<Result<TResult>>>? nextCommandFactory,
    string title)
    : IStepDefinition
{
    private TPrevious previousResult = default!;

    /// <inheritdoc />
    public string Title { get; } = title;

    /// <inheritdoc />
    public object CreatePage(object? previousResult)
    {
        this.previousResult = previousResult is null ? default! : (TPrevious)previousResult;
        return pageFactory(this.previousResult);
    }

    /// <inheritdoc />
    public IEnhancedCommand<Result<object>>? GetNextCommand(object page)
    {
        if (nextCommandFactory == null)
            return null;

        var typedPage = (TPage)page;
        var typedCommand = nextCommandFactory(typedPage, previousResult);
        return new CommandAdapter<Result<TResult>, Result<object>>(typedCommand, result => result.Map(x => (object)x));
    }
}
