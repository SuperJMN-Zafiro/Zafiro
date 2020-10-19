using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using ReactiveUI;
using Zafiro.Core;

namespace Zafiro.Wpf.Services.MarkupWindow
{
    public class DialogViewModel : ReactiveObject
    {
        public DialogViewModel(object content, ICollection<DialogButton> options, IPopup popup) : this("",
            content, options, popup)
        {
        }

        public DialogViewModel(string title, object content, ICollection<DialogButton> buttons, IPopup popup)
        {
            Title = title;
            Content = content;

            CloseCommand = ReactiveCommand.Create(popup.Close);

            Buttons = buttons.Select(button =>
            {
                return new Core.OptionViewModel(button,
                        ReactiveCommand.Create<Unit>(unit => button.Handler(popup), button.CanExecute));
            }).ToList();
        }

        public ReactiveCommand<Unit, Unit> CloseCommand { get; }


        public string Title { get; }
        public object Content { get; set; }

        public ICollection<Core.OptionViewModel> Buttons { get; set; }
    }
}