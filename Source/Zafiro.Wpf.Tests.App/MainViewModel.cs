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
            ShowMessage = ReactiveCommand.CreateFromTask(() => dialogService.Message("Bendito", File.ReadAllText("Files\\Readme.md"), "OK".Some(), @"D:\Repos\SuperJMN-Zafiro\Zafiro\Source\Zafiro.Wpf.Tests.App\bin\Debug\netcoreapp3.1\Files".Some()));
        }

        public ReactiveCommand<Unit, Unit> ShowMessage { get; set; }
    }
}