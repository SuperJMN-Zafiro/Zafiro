using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.Tests.UI;

public record Page(object Content, IEnhancedCommand<Result<object>>? NextCommand, string Title);