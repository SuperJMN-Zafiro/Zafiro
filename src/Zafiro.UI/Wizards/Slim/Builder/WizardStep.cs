using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim.Builder;

public class WizardStep : IWizardStep
{
    private readonly Func<object?, object> createPage;
    private readonly Func<object, IEnhancedCommand<Result<object>>?> getNextCommand;

    public WizardStep(StepKind kind, string title, Func<object?, object> createPage, Func<object, IEnhancedCommand<Result<object>>?> getNextCommand)
    {
        this.createPage = createPage;
        this.getNextCommand = getNextCommand;
        Kind = kind;
        Title = title;
    }

    public string Title { get; }
    public object CreatePage(object? previousResult)
    {
        return createPage(previousResult);
    }

    public IEnhancedCommand<Result<object>>? GetNextCommand(object page)
    {
        return getNextCommand(page);
    }

    public StepKind Kind { get; }
}