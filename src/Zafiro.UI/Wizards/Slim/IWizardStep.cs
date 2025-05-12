using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim;

public interface IWizardStep
{
    string Title { get; }
    object CreatePage(object? previousResult);
    IEnhancedCommand<Result<object>>? GetNextCommand(object page);
}