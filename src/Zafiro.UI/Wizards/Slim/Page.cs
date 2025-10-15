using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim;

/// <summary>
/// Default page implementation used internally by SlimWizard.
/// </summary>
public record Page(int Index, object Content, IEnhancedCommand<Result<object>> NextCommand, string Title) : IPage;
