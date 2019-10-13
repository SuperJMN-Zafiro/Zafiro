using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Zafiro.Core;

namespace Zafiro.Uwp.Controls.ObjEditor
{
    public class ObjectEditor : Control
    {
        public ObjectEditor()
        {
            DefaultStyleKey = typeof(ObjectEditor);
        }

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems", typeof(object), typeof(ObjectEditor), new PropertyMetadata(default(object), OnSelectedItemsChanged));

        private static void OnSelectedItemsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var target = (ObjectEditor)dependencyObject;
            target.OnSelectedItemsChanged(dependencyPropertyChangedEventArgs.OldValue,
                dependencyPropertyChangedEventArgs.NewValue);
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

        public static readonly DependencyProperty PropertyItemsProperty = DependencyProperty.Register(
            "PropertyItems", typeof(IList<PropertyItem>), typeof(ObjectEditor), new PropertyMetadata(default(IList<PropertyItem>), SelectedItemsChanged));

        private static void SelectedItemsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var target = (ObjectEditor)dependencyObject;
            target.OnPropertyItemsChanged((IList<PropertyItem>)args.OldValue, (IList<PropertyItem>)args.NewValue);
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

        public static readonly DependencyProperty EditorsProperty = DependencyProperty.Register(
            "Editors", typeof(EditorCollection), typeof(ObjectEditor), new PropertyMetadata(new EditorCollection()));

        public EditorCollection Editors
        {
            get { return (EditorCollection)GetValue(EditorsProperty); }
            set { SetValue(EditorsProperty, value); }
        }

        public static readonly DependencyProperty DefaultEditorTemplateProperty = DependencyProperty.Register(
            "DefaultEditorTemplate", typeof(DataTemplate), typeof(ObjectEditor), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate DefaultEditorTemplate
        {
            get { return (DataTemplate) GetValue(DefaultEditorTemplateProperty); }
            set { SetValue(DefaultEditorTemplateProperty, value); }
        }
    }
}