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
using Zafiro.Core;

namespace Zafiro.Uwp.Controls.ObjEditor
{
    public class GroupGetter
    {
        private readonly PropertyInfo property;

        public GroupGetter(PropertyInfo property)
        {
            this.property = property;
        }

        public object GetValue(IEnumerable<object> targets)
        {
            var query = from target in targets
                from prop in target.GetType().GetRuntimeProperties().Where(x => string.Equals(x.Name, property.Name))
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

            return property.PropertyType.IsValueType ? Activator.CreateInstance(property.PropertyType) : null;
        }
    }

    public class PropertyItem : Control, INotifyPropertyChanged, IDisposable
    {
        private readonly PropertyInfo property;
        private readonly IList<object> targets;
        private readonly IList<INotifyPropertyChanged> observables;
        private readonly GroupSetter groupSetter;
        private readonly GroupGetter groupGetter;

        private PropertyItem()
        {
            DefaultStyleKey = typeof(PropertyItem);
        }

        public PropertyItem(PropertyInfo property, IList<object> targets) : this()
        {
            this.property = property ?? throw new ArgumentNullException(nameof(property));
            this.targets = targets ?? throw new ArgumentNullException(nameof(targets));

            groupSetter = new GroupSetter(property);
            groupGetter = new GroupGetter(property);
            observables = targets.OfType<INotifyPropertyChanged>().ToList();
            Subscribe(observables);
        }

        public Type PropType => property.PropertyType;
        public string PropName => property.Name;

        public object Value
        {
            get => groupGetter.GetValue(targets);
            set => groupSetter.Set(targets, value);
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
            return Activator.CreateInstance(property.PropertyType);
        }

        public bool IsExpandable => property.Name == "Shadow";

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