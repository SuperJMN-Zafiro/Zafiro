namespace Zafiro.UI.Navigation.Sections;

public interface INamedSection : ISection
{
    public string Name { get; }
    object? Icon { get; }
}