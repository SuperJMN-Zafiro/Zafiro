using Avalonia;
using Avalonia.Markup.Xaml;

namespace SampleApp
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
