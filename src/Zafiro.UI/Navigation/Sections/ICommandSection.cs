using System.Windows.Input;

namespace Zafiro.UI.Navigation.Sections;

public interface ICommandSection : INamedSection
{
    ICommand Command { get; }
}