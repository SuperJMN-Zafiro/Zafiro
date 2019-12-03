using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using ReactiveUI;
using Zafiro.Core.ObjectEditor;
using Zafiro.Core.Values;

namespace Zafiro.Uwp.ObjectEditor
{
    public class PropertyItem : PropertyItem<FrameworkElement>
    {
        public PropertyItem(FrameworkElement valueEditor, PropertyInfo propertyInfo, IEnumerable<object> targets) : base(valueEditor, propertyInfo, targets)
        {
        }
    }
}