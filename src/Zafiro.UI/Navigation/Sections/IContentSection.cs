namespace Zafiro.UI.Navigation.Sections;

public interface IContentSection : INamedSection
{
    Func<object?> GetViewModel { get; }
    object? Content { get; }
}