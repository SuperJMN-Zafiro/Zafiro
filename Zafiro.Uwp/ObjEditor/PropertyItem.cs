using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Zafiro.Core.Values;

namespace Zafiro.Uwp.Controls.ObjEditor
{
    public class PropertyItem : Control, INotifyPropertyChanged, IDisposable
    {
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly GroupGetter groupGetter;
        private readonly GroupSetter groupSetter;
        private readonly PropertyInfo property;
        private readonly IList<object> targets;

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
            var observables = targets.OfType<INotifyPropertyChanged>().ToList();
            SubscribeToPropertyChangesOf(observables);
        }

        public Type PropertyType => property.PropertyType;

        public string PropertyName => property.Name;

        public object Value
        {
            get => groupGetter.GetValue(targets);
            set => groupSetter.Set(targets, value);
        }

        public FrameworkElement Editor => CreateEditor(this);

        public bool IsExpandable => property.Name == "Shadow";

        public void Dispose()
        {
            disposables.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private FrameworkElement CreateEditor(PropertyItem propertyItem)
        {
            if (IsExpandable)
            {
                return CreateExpandableEditor();
            }

            var objectEditor = propertyItem.FindAscendant<ObjectEditor>();
            var editorTemplates = objectEditor.Editors;
            var template = editorTemplates
                               .FirstOrDefault(e => IsMatch(propertyItem, e))?.Template ??
                           objectEditor.DefaultEditorTemplate;

            return (FrameworkElement) template.LoadContent();
        }

        private static bool IsMatch(PropertyItem propertyItem, Editor e)
        {
            var matchesPropName = e.Key.PropertyName == null || e.Key.PropertyName == propertyItem.PropertyName;
            var matchesType = e.Key.TargetType == propertyItem.PropertyType;
            return matchesType && matchesPropName;
        }

        private FrameworkElement CreateExpandableEditor()
        {
            return new ObjectEditor
            {
                SelectedItems = Value ?? CreateNewInstance()
            };
        }

        private object CreateNewInstance()
        {
            return Activator.CreateInstance(property.PropertyType);
        }

        private void SubscribeToPropertyChangesOf(IEnumerable<INotifyPropertyChanged> observables)
        {
            var subscriptions = from observable in observables
                let subscription =
                    Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                            h => observable.PropertyChanged += h, h => PropertyChanged -= h)
                        .Subscribe(args => ObsOnPropertyChanged(args.EventArgs.PropertyName))
                select subscription;

            foreach (var subscription in subscriptions)
            {
                disposables.Add(subscription);
            }
        }

        private void ObsOnPropertyChanged(string propertyName)
        {
            if (PropertyName == propertyName)
            {
                UpdateValue();
            }
        }

        private void UpdateValue()
        {
            OnPropertyChanged(nameof(Value));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}