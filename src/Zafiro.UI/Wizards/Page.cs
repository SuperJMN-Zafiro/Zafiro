using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards;

public record Page(object Content, IEnhancedCommand<Result<object>>? NextCommand, string Title);