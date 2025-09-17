using System.ComponentModel;

namespace Zafiro.UI.Navigation.Sections;

public interface INamedSection : ISection, INotifyPropertyChanged
{
    public string Name { get; }
    public string FriendlyName { get; }
    object? Icon { get; }
}