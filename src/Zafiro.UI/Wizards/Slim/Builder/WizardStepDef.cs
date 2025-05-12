using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim.Builder;

public class WizardStepDef<TPage, TResult>(
    Func<object?, TPage> pageFactory,
    Func<TPage, IEnhancedCommand<Result<TResult>>>? nextCommandFactory,
    string title)
    : IWizardStepDef
{
    public string Title { get; } = title;

    public object CreatePage(object? previousResult)
    {
        return pageFactory(previousResult);
    }

    public IEnhancedCommand<Result<object>>? GetNextCommand(object page)
    {
        if (nextCommandFactory == null)
            return null;

        var typedPage = (TPage)page;
        var typedCommand = nextCommandFactory(typedPage);
        return new CommandAdapter<Result<TResult>, Result<object>>(typedCommand, result => result.Map(x => (object)x));
    }
}