namespace Zafiro.UI.Navigation.Sections;

public interface INamedSection : ISection
{
    public string Name { get; }
    public string FriendlyName { get; }
    object? Icon { get; }
}