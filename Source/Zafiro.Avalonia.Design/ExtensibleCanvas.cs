using System.Linq;
using Avalonia;
using Avalonia.Controls;

namespace Zafiro.Avalonia.Design
{
    public class ExtensibleCanvas : Canvas
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in Children)
            {
                child.Measure(availableSize);
            }
            
            var frameworkElements = Children.OfType<Control>().ToList();

            if (frameworkElements.Count == 0)
            {
                return new Size(0, 0);
            }

            var w = frameworkElements.Max(o => GetLeft(o) + o.DesiredSize.Width);
            var h = frameworkElements.Max(o => GetTop(o) + o.DesiredSize.Height);

            return new Size(w, h);
        }
    }
}