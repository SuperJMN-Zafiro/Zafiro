using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Templates;
using Zafiro.Core;

namespace Zafiro.Avalonia.ObjEditor
{
    public class ObjectEditor : TemplatedControl
    {
        public static readonly AvaloniaProperty SelectedItemsProperty = AvaloniaProperty.Register<ObjectEditor, object>(
            "SelectedItems", null, false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty PropertyItemsProperty =
            AvaloniaProperty.Register<ObjectEditor, IList<PropertyItem>>(
                "PropertyItems", default, false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty EditorsProperty =
            AvaloniaProperty.Register<ObjectEditor, EditorCollection>(
                "Editors", new EditorCollection(), false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty DefaultEditorTemplateProperty =
            AvaloniaProperty.Register<ObjectEditor, DataTemplate>(
                "Editors", null, false, BindingMode.TwoWay);

        private CompositeDisposable disposables = new CompositeDisposable();

        public ObjectEditor()
        {
            SelectedItemsProperty.Changed.Subscribe(args => OnSelectedItemsChanged(args.OldValue, args.NewValue));
            PropertyItemsProperty.Changed.Subscribe(args =>
                OnPropertyItemsChanged((IList<PropertyItem>) args.OldValue, (IList<PropertyItem>) args.NewValue));
        }

        public IList<PropertyItem> PropertyItems
        {
            get => (IList<PropertyItem>) GetValue(PropertyItemsProperty);
            set => SetValue(PropertyItemsProperty, value);
        }

        public object SelectedItems
        {
            get => GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public EditorCollection Editors
        {
            get => (EditorCollection) GetValue(EditorsProperty);
            set => SetValue(EditorsProperty, value);
        }

        public DataTemplate DefaultEditorTemplate
        {
            get => (DataTemplate) GetValue(DefaultEditorTemplateProperty);
            set => SetValue(DefaultEditorTemplateProperty, value);
        }

        private void OnSelectedItemsChanged(object oldValue, object newValue)
        {
            disposables.Dispose();
            disposables = new CompositeDisposable();

            IList<object> targets;
            if (newValue is IList<object> list)
            {
                targets = list;
                if (list is INotifyCollectionChanged col)
                {
                    Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                            h => col.CollectionChanged += h, h => col.CollectionChanged -= h)
                        .Subscribe(x => OnCollectionChanged())
                        .DisposeWith(disposables);
                }
            }
            else
            {
                targets = new List<object> {newValue};
            }

            PropertyItems = UpdatePropertyItems(targets);

            foreach (var propertyItem in PropertyItems)
            {
                disposables.Add(propertyItem);
            }
        }

        private static List<PropertyItem> UpdatePropertyItems(IList<object> targets)
        {
            if (targets == null || targets.Any(o => o == null))
            {
                return new List<PropertyItem>();
            }

            var allProperties = targets
                .Select(o =>
                    o.GetType().GetRuntimeProperties()
                        .Where(p => p.GetMethod != null && p.SetMethod != null)
                        .Where(p => !(p.GetMethod.IsStatic || p.SetMethod.IsStatic))
                        .Where(p => p.SetMethod.IsPublic && p.GetMethod.IsPublic)
                        .Where(x => x.GetCustomAttribute<HiddenAttribute>() == null))
                .ToList();

            if (allProperties.Count == 0)
            {
                return new List<PropertyItem>();
            }

            var equalityComparer =
                new LambdaComparer<PropertyInfo>((a, b) => a.PropertyType == b.PropertyType && a.Name == b.Name);
            var commonProperties = allProperties.Count == 1
                ? allProperties.First()
                : allProperties.GetCommon(equalityComparer);

            return commonProperties.Select(o => new PropertyItem(o, targets))
                .OrderBy(item => item.PropertyName)
                .ToList();
        }

        private void OnCollectionChanged()
        {
            UpdatePropertyItems((IList<object>) SelectedItems);
        }

        private void OnPropertyItemsChanged(IList<PropertyItem> oldValue, IList<PropertyItem> newValue)
        {
            if (oldValue == null)
            {
                return;
            }

            foreach (var i in oldValue)
            {
                i.Dispose();
            }
        }
    }
}