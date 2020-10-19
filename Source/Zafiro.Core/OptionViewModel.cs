using System.Reactive;
using ReactiveUI;

namespace Zafiro.Core
{
    public class OptionViewModel
    {
        public DialogButton Button { get; }
        public ReactiveCommand<Unit, Unit> Command { get; }

        public OptionViewModel(DialogButton button, ReactiveCommand<Unit, Unit> command)
        {
            Button = button;
            Command = command;
        }
    }
}