using System.Windows.Input;

namespace Zafiro.UI.Navigation.Sections;

public class CommandSection(string name, ICommand command, object? icon) : SectionBase
{
    public string Name { get; } = name;
    public ICommand Command { get; } = command;
    public object? Icon { get; } = icon;
}