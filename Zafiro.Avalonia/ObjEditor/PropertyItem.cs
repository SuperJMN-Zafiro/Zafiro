using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace Zafiro.Avalonia.ObjEditor
{
    public class PropertyItem : Control, INotifyPropertyChanged, IDisposable
    {
        private readonly IList<object> targets;
        private readonly IList<INotifyPropertyChanged> observables;

        public PropertyItem(Type propType, string propName, IList<object> targets)
        {
            this.targets = targets;
            PropType = propType;
            PropName = propName;

            observables = targets.OfType<INotifyPropertyChanged>().ToList();
            Subscribe(observables);
        }

        public Type PropType { get; }
        public string PropName { get; }

        public object Value
        {
            get
            {
                var values = targets.Select(o => o.GetType().GetProperty(PropName).GetValue(o)).ToList();
                if (values.Distinct().Count() == 1)
                {
                    return values.First();
                }

                return (PropType.IsValueType) ? Activator.CreateInstance(this.PropType) : null;
            }
            set
            {
                foreach (var target in targets)
                {
                    object finalValue;
                    if (!PropType.IsInstanceOfType(value))
                    {
                        finalValue = TypeDescriptor.GetConverter(PropType).ConvertFrom(value);
                    }
                    else
                    {
                        finalValue = value;
                    }

                    var propertyInfo = target.GetType().GetProperty(PropName);
                    propertyInfo.SetValue(target, finalValue);

                    ReplicateValueToParentObjectIfAny(propertyInfo, finalValue);
                }
            }
        }

        private void ReplicateValueToParentObjectIfAny(PropertyInfo propertyInfo, object finalValue)
        {
            var objectEditor = this.FindAscendant<ObjectEditor>();

            if (objectEditor != null)
            {
                var parent = objectEditor.FindAscendant<PropertyItem>();
                if (parent != null)
                {
                    foreach (var obs in parent.observables)
                    {
                        var rootInstance = obs.GetType().GetRuntimeProperty(parent.PropName).GetValue(obs);
                        propertyInfo.SetValue(rootInstance, finalValue);
                    }
                }
            }
        }

        public Control Editor => CreateEditor(this);

        private Control CreateEditor(PropertyItem propertyItem)
        {
            if (IsExpandable)
            {
                return CreateExpandableEditor();
            }

            var objectEditor = propertyItem.FindAscendant<ObjectEditor>();
            var editorTemplates = objectEditor.Editors;
            var template = editorTemplates
                .FirstOrDefault(e => IsMatch(propertyItem, e))?.Template ?? objectEditor.DefaultEditorTemplate;

            return (Control)template.Build(propertyItem);
        }

        private static bool IsMatch(PropertyItem propertyItem, Editor e)
        {
            var matchesPropName = e.Key.PropertyName == null || e.Key.PropertyName == propertyItem.PropName;
            var matchesType = e.Key.TargetType == propertyItem.PropType;
            return matchesType && matchesPropName;
        }

        private Control CreateExpandableEditor()
        {
            return new ObjectEditor
            {
                SelectedItems = Value ?? CreateNewInstance(),
            };
        }

        private object CreateNewInstance()
        {
            return Activator.CreateInstance(PropType);
        }

        public bool IsExpandable => PropName == "Shadow";

        private void Subscribe(IEnumerable<INotifyPropertyChanged> observables)
        {
            foreach (var obs in observables)
            {
                obs.PropertyChanged += ObsOnPropertyChanged;
            }
        }

        private void Unsubscribe(IEnumerable<INotifyPropertyChanged> observables)
        {
            foreach (var obs in observables)
            {
                obs.PropertyChanged -= ObsOnPropertyChanged;
            }
        }

        private void ObsOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (PropName == propertyChangedEventArgs.PropertyName)
            {
                UpdateValue();
            }
        }

        private void UpdateValue()
        {
            OnPropertyChanged(nameof(Value));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            Unsubscribe(observables);
        }
    }

    public static class VisualTreeExtensions 
    {
        public static T FindAscendant<T>(this Control self) where T : IVisual
        {
            return self.GetVisualAncestors().OfType<T>().First();
        }
    }
}
