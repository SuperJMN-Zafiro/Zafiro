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
            var route =
                "C:\\Users\\JMN\\Extended\\Fast\\Repos\\WOA-Project\\Deployment-Feed\\Devices\\Lumia\\950s\\Cityman\\Packages\\Changelog";
            var dialogService = new Interaction(new UI.Popup(() => new PopupWindow()));
            ShowMessage = ReactiveCommand.CreateFromTask(() => dialogService.Message("Bendito", File.ReadAllText(Path.Combine(route, "Changelog.md")), "OK".Some(), route.Some()));
        }

        public ReactiveCommand<Unit, Unit> ShowMessage { get; set; }
    }
}