using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Zafiro.Core;

namespace Zafiro.Wpf.Services.MarkupWindow
{
    public class DialogViewModel : ReactiveObject, IDisposable
    {
        private readonly IDisposable closer;

        public DialogViewModel(object content, ICollection<DialogButton> options, ICloseable closeable) : this("",
            content, options, closeable)
        {
        }

        public DialogViewModel(string title, object content, ICollection<DialogButton> buttons, ICloseable closeable)
        {
            Title = title;
            Content = content;

            CloseCommand = ReactiveCommand.Create(() =>
            {
                closeable.Close();
                closer.Dispose();
            });

            Buttons = buttons.Select(button =>
            {
                return new OptionViewModel2(button,
                        ReactiveCommand.Create<Unit>(unit => button.Handler(closeable), button.CanExecute));
            }).ToList();
            closer = Buttons.Select(x => x.Command).Merge().InvokeCommand(CloseCommand);
        }

        public ReactiveCommand<Unit, Unit> CloseCommand { get; }


        public string Title { get; }
        public object Content { get; set; }

        public ICollection<OptionViewModel2> Buttons { get; set; }

        public void Dispose()
        {
            closer?.Dispose();
        }
    }
}