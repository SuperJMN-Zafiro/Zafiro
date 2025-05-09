using CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards;

public interface IPage
{
    object Content { get; }
    string Title { get; }
    public int Index { get; }
}

public record Page(int Index, object Content, IEnhancedCommand<Result<object>>? NextCommand, string Title) : IPage;