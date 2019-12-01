using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace SampleApp.DesignerSurfaceDemo
{
    public class Window1 : Window
    {
        public Window1()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools(KeyGesture.Parse("F1"));
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
