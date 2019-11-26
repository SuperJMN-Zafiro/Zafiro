using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Serilog;

namespace Zafiro.Uwp.Controls.ObjEditor
{
    public class PropertyItem : Control, INotifyPropertyChanged, IDisposable
    {
        private readonly PropertyInfo property;
        private readonly IList<object> targets;
        private readonly IList<INotifyPropertyChanged> observables;

        private PropertyItem()
        {
            DefaultStyleKey = typeof(PropertyItem);
        }

        public PropertyItem(PropertyInfo property, IList<object> targets) : this()
        {
            this.property = property ?? throw new ArgumentNullException(nameof(property));
            this.targets = targets ?? throw new ArgumentNullException(nameof(targets));

            observables = targets.OfType<INotifyPropertyChanged>().ToList();
            Subscribe(observables);
        }

        public Type PropType => property.PropertyType;
        public string PropName => property.Name;

        public object Value
        {
            get => GetValue();
            set => SetValue(this, value, targets, property);
        }

        private static void SetValue(PropertyItem parent, object value, IEnumerable<object> objects, PropertyInfo property)
        {
            try
            {
                foreach (var target in objects)
                {
                    object finalValue;
                    if (!property.PropertyType.IsInstanceOfType(value))
                    {
                        var typeConverter = TypeDescriptor.GetConverter(property.PropertyType);
                        if (typeConverter.CanConvertFrom(value.GetType()))
                        {
                            finalValue = typeConverter.ConvertFrom(value);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        finalValue = value;
                    }

                    property.SetValue(target, finalValue);

                    ReplicateValueToParentObjectIfAny(parent, property, finalValue);
                }
            }
            catch (Exception e)
            {
                Log.Warning(e, "Could not set property {PropName} to value {Value}", property.Name, value);
            }
        }

        private object GetValue()
        {
            var query = from target in targets
                        from prop in target.GetType().GetProperties().Where(x => x.Name == PropName)
                        select new { target, prop };

            var values = query.Select(x =>
            {
                try
                {
                    var value = x.prop.GetValue(x.target);
                    return value;
                }
                catch (Exception e)
                {
                    Log.Warning(e, "Could not get values of property {Property}", x);
                    return null;
                }
            }).ToList();

            if (values.Distinct().Count() == 1)
            {
                return values.First();
            }

            return (PropType.IsValueType) ? Activator.CreateInstance(this.PropType) : null;
        }

        private static void ReplicateValueToParentObjectIfAny(PropertyItem parentEditor, PropertyInfo propertyInfo, object finalValue)
        {
            var objectEditor = parentEditor.FindAscendant<ObjectEditor>();

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

        public FrameworkElement Editor => CreateEditor(this);

        private FrameworkElement CreateEditor(PropertyItem propertyItem)
        {
            if (IsExpandable)
            {
                return CreateExpandableEditor();
            }

            var objectEditor = propertyItem.FindAscendant<ObjectEditor>();
            var editorTemplates = objectEditor.Editors;
            var template = editorTemplates
                .FirstOrDefault(e => IsMatch(propertyItem, e))?.Template ?? objectEditor.DefaultEditorTemplate;

            return (FrameworkElement)template.LoadContent();
        }

        private static bool IsMatch(PropertyItem propertyItem, Editor e)
        {
            var matchesPropName = e.Key.PropertyName == null || e.Key.PropertyName == propertyItem.PropName;
            var matchesType = e.Key.TargetType == propertyItem.PropType;
            return matchesType && matchesPropName;
        }

        private FrameworkElement CreateExpandableEditor()
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
}