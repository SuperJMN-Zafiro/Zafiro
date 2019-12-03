using System.Collections.Generic;
using System.Reflection;
using Avalonia.Controls;
using Zafiro.Core.ObjectEditor;

namespace Zafiro.Avalonia.ObjectEditor
{
    public class PropertyItem : PropertyItem<Control>
    {
        public PropertyItem(Control valueEditor, PropertyInfo propertyInfo, IEnumerable<object> targets) : base(valueEditor, propertyInfo, targets)
        {
        }
    }
}