using System.Windows;
using Zafiro.Core;

namespace Zafiro.Wpf.Services
{
    public class WpfContextualizable : IContextualizable
    {
        private readonly FrameworkElement frameworkElement;

        public WpfContextualizable(FrameworkElement frameworkElement)
        {
            this.frameworkElement = frameworkElement;
        }

        public void SetContext(object o)
        {
            frameworkElement.DataContext = o;
        }

        public object Object => frameworkElement;
    }
}