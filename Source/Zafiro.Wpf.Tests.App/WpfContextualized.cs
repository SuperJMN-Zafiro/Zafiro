using System.Windows;
using Zafiro.Core;

namespace Zafiro.Wpf.Tests.App
{
    internal class WpfContextualized : IContextualizable
    {
        private readonly FrameworkElement content;

        public WpfContextualized(FrameworkElement content)
        {
            this.content = content;
        }

        public void SetContext(object o)
        {
            content.DataContext = o;
        }

        public object Object => content;
    }
}