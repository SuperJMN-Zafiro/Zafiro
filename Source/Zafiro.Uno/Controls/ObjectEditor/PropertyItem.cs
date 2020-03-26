using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml;
using Zafiro.Core.ObjectEditor;

namespace Zafiro.Uno.Controls.ObjectEditor
{
    public class PropertyItem : PropertyItem<FrameworkElement>
    {
        public PropertyItem(FrameworkElement valueEditor, PropertyInfo propertyInfo, IEnumerable<object> targets) : base(valueEditor, propertyInfo, targets)
        {
        }
    }
}