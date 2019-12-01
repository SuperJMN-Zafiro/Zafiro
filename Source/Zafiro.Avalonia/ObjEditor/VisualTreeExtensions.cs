using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace Zafiro.Avalonia.ObjEditor
{
    public static class VisualTreeExtensions 
    {
        public static T FindAscendant<T>(this Control self) where T : IVisual
        {
            return self.GetVisualAncestors().OfType<T>().FirstOrDefault();
        }
    }
}