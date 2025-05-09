using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards;

public class WizardStep<TPage, TResult> : IWizardStep
{
    private readonly Func<TPage, IEnhancedCommand<Result<TResult>>>? nextCommandFactory;
    private readonly Func<object?, TPage> pageFactory;

    public WizardStep(
        Func<object?, TPage> pageFactory,
        Func<TPage, IEnhancedCommand<Result<TResult>>>? nextCommandFactory,
        string title,
        string nextText)
    {
        this.pageFactory = pageFactory;
        this.nextCommandFactory = nextCommandFactory;
        Title = title;
        NextText = nextText;
    }

    public string NextText { get; }
    public string Title { get; }

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

public interface IWizardStep
{
    string NextText { get; }
    string Title { get; }
    object CreatePage(object? previousResult);
    IEnhancedCommand<Result<object>>? GetNextCommand(object page);
}