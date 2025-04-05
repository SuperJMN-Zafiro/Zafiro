using System.Windows.Input;
using CSharpFunctionalExtensions;

namespace Zafiro.UI.Navigation.Sections;

public class CommandSection(string name, ICommand command, object? icon) : Section, ICommandSection
{
    public string Name { get; } = name;
    public ICommand Command { get; } = command;
    public object? Icon { get; } = icon;
}