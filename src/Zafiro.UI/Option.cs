using System.Windows.Input;

namespace Zafiro.UI;

public class Option
{
    public Option(string title, ICommand command)
    {
        Title = title;
        Command = command;
    }

    public string Title { get; }
    public ICommand Command { get; }
}