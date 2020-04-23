using System.Reactive;
using System.Windows.Input;
using ReactiveUI;

namespace Zafiro.Core
{
    public class OptionViewModel2
    {
        public DialogButton Button { get; }
        public ReactiveCommand<Unit, Unit> Command { get; }

        public OptionViewModel2(DialogButton button, ReactiveCommand<Unit, Unit> command)
        {
            Button = button;
            Command = command;
        }
    }
}