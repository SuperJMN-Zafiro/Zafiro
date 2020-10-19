using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using ReactiveUI;
using Zafiro.Core.UI.Interaction;
using Zafiro.Wpf.Services;
using Zafiro.Wpf.Services.MarkupWindow;
using Option = Zafiro.Core.UI.Interaction.Option;

namespace Zafiro.Wpf.Tests.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var shell = new Shell(() => new PopupWindow());
            var content = new Label();
            content.SetBinding(Label.ContentProperty, "Text");
            await shell.Popup(new WpfContextualized(content), new ViewModel(), configuration =>
            {
                configuration.Popup.Title = "Title";
                configuration.AddOption(new Option("OK", ReactiveCommand.Create(() => configuration.Popup.Close())));
                configuration.AddOption(new Option("Change content", ReactiveCommand.Create(() => configuration.Model.Text = "Changed")));
            });
        }
    }

    public static class ConfigExtensions
    {
        public static PopupConfiguration<T> RegularOption<T>(this PopupConfiguration<T> config, string title, ICommand command)
        {
            config.AddOption(new Option(title, command));

            return config;
        }
    }

    internal class ViewModel : ReactiveObject
    {
        private string text;

        public ViewModel()
        {
            Text = "Salute";
        }

        public string Text
        {
            get => text;
            set => this.RaiseAndSetIfChanged(ref text, value);
        }
    }
}
