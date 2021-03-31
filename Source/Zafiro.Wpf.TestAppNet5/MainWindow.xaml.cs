using System.IO;
using System.Windows;
using Optional;
using Zafiro.UI;
using Zafiro.UI.Wpf;

namespace Zafiro.Wpf.TestAppNet5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var route =
                "C:\\Users\\JMN\\Extended\\Fast\\Repos\\WOA-Project\\Deployment-Feed\\Devices\\Lumia\\950s\\Cityman\\Packages\\Changelog";
            var markdown = File.ReadAllText(Path.Combine(route, "Changelog.md"));

            var i = new Interaction(new Popup(() => new PopupWindow()));
            await i.Message("Saludos", markdown, "".Some(), route.Some());
        }
    }
}
