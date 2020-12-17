using System.Windows;

namespace Zafiro.UI.Wpf
{
    public class HaveDataContextAdapter : IHaveDataContext
    {
        private readonly FrameworkElement frameworkElement;

        public HaveDataContextAdapter(FrameworkElement frameworkElement)
        {
            this.frameworkElement = frameworkElement;
        }

        public void SetDataContext(object dataContext)
        {
            frameworkElement.DataContext = dataContext;
        }

        public object Object => frameworkElement;
    }
}