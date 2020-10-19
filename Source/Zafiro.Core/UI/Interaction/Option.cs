using System.Windows.Input;

namespace Zafiro.Core.UI.Interaction
{
    public class Option
    {
        public string Title { get; }
        public ICommand Command { get; }

        public Option(string title, ICommand command)
        {
            Title = title;
            Command = command;
        }
    }
}