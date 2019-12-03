using System;
using System.Collections.Generic;
using System.Reflection;
using ReactiveUI;
using Zafiro.Core.ObjectEditor.TemplateMatchers;

namespace Zafiro.Core.ObjectEditor
{
    public abstract class PropertyItem<T> : ReactiveObject, IDisposable
    {
        private readonly T valueEditor;
        public PropertyInfo PropertyInfo { get; }
        public IList<object> Targets { get; }

        public PropertyItem(T valueEditor, PropertyInfo propertyInfo, IList<object> targets)
        {
            this.valueEditor = valueEditor;
            this.PropertyInfo = propertyInfo;
            this.Targets = targets;
        }

        public object PropertyName => PropertyInfo.Name;


        public abstract void Dispose();
    }
}