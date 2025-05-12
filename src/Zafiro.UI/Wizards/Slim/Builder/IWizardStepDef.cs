using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim.Builder;

public interface IWizardStepDef
{
    string Title { get; }
    object CreatePage(object? previousResult);
    IEnhancedCommand<Result<object>>? GetNextCommand(object page);
}