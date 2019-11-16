using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Templates;
using Zafiro.Core;

namespace Zafiro.Avalonia.ObjEditor
{
    public class ObjectEditor : Control
    {
        public ObjectEditor()
        {
            
            SelectedItemsProperty.Changed.Subscribe(args => OnSelectedItemsChanged(args.OldValue, args.NewValue));
            PropertyItemsProperty.Changed.Subscribe(args => OnPropertyItemsChanged((IList<PropertyItem>) args.OldValue, (IList<PropertyItem>) args.NewValue));
        }

        public static readonly AvaloniaProperty SelectedItemsProperty = AvaloniaProperty.Register<ObjectEditor, object>(
            "SelectedItems", null, false, BindingMode.OneWayToSource);

        public static readonly AvaloniaProperty PropertyItemsProperty = AvaloniaProperty.Register<ObjectEditor, IList<PropertyItem>>(
            "PropertyItems", default, false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty EditorsProperty = AvaloniaProperty.Register<ObjectEditor, EditorCollection>(
            "Editors", new EditorCollection(), false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty DefaultEditorTemplateProperty = AvaloniaProperty.Register<ObjectEditor, DataTemplate>(
            "Editors", null, false, BindingMode.TwoWay);

        public EditorCollection Editors
        {
            get { return (EditorCollection)GetValue(EditorsProperty); }
            set { SetValue(EditorsProperty, value); }
        }

        private void OnSelectedItemsChanged(object oldValue, object newValue)
        {
            IList<object> targets;
            if (newValue is IList<object> list)
            {
                targets = list;
                if (list is INotifyCollectionChanged col)
                {
                    col.CollectionChanged += OnCollectionChanged;
                }
            }
            else
            {
                targets = new List<object> { newValue };
            }

            UpdatePropertyItems(targets);
        }

        private void UpdatePropertyItems(IList<object> targets)
        {
            if (targets == null || targets.Any(o => o == null))
            {
                return;
            }

            var keyedProperies = targets
                .Select(o =>
                    o.GetType().GetRuntimeProperties()
                        .Where(p => p.GetMethod != null && p.SetMethod != null)
                        .Where(p => !(p.GetMethod.IsStatic || p.SetMethod.IsStatic))
                        .Where(p => p.SetMethod.IsPublic && p.GetMethod.IsPublic)
                        .Where(x => x.GetCustomAttribute<HiddenAttribute>() == null)
                        .Select(p => new KeyedProperty { Key = (p.PropertyType, p.Name), Property = p }))
                .ToList();

            if (keyedProperies.Count == 0)
            {
                PropertyItems = null;
            }
            else
            {
                var comparer = new LambdaComparer<KeyedProperty>((a, b) => a.Key.Equals(b.Key));
                var properties = keyedProperies.Count == 1 ? keyedProperies.First() : keyedProperies.GetCommon(comparer);

                PropertyItems = properties.Select(o => new PropertyItem(o.Key.Item1, o.Key.Item2, targets))
                    .OrderBy(item => item.PropName)
                    .ToList();
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePropertyItems((IList<object>)SelectedItems);
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

        public IList<PropertyItem> PropertyItems
        {
            get { return (IList<PropertyItem>)GetValue(PropertyItemsProperty); }
            set { SetValue(PropertyItemsProperty, value); }
        }

        public object SelectedItems
        {
            get { return (object)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        private class KeyedProperty
        {
            public (Type, string) Key { get; set; }
            public PropertyInfo Property { get; set; }
        }

        public DataTemplate DefaultEditorTemplate
        {
            get { return (DataTemplate)GetValue(DefaultEditorTemplateProperty); }
            set { SetValue(DefaultEditorTemplateProperty, value); }
        }
    }
}