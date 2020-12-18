using System.IO;
using System.Reactive;
using Optional;
using ReactiveUI;
using Zafiro.UI.Wpf;

namespace Zafiro.Wpf.Tests.App
{
    internal class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            var dialogService = new Interaction(new UI.Popup(() => new PopupWindow()));
            ShowMessage = ReactiveCommand.CreateFromTask(() => dialogService.Message("Bendito", "Sea el señor", "OK".Some(), Option.None<string>()));
        }

        public ReactiveCommand<Unit, Unit> ShowMessage { get; set; }
    }
}