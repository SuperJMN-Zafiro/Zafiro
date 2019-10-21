using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Zafiro.Uwp.Designer
{
    public class ExtensibleCanvas : Canvas
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in Children)
            {
                child.Measure(availableSize);
            }


            var frameworkElements = Children.OfType<FrameworkElement>().ToList();

            if (frameworkElements.Count == 0)
            {
                return new Size(0, 0);
            }

            var w = frameworkElements.Max(e => (double)e.GetValue(LeftProperty) + e.ActualWidth);
            var h = frameworkElements.Max(e => (double)e.GetValue(TopProperty) + e.ActualHeight);

            return new Size(w, h);
        }
    }
}
