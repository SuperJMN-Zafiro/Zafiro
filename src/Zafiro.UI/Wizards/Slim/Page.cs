using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim;

public record Page(int Index, object Content, IEnhancedCommand<Result<object>> NextCommand, string Title) : IPage;