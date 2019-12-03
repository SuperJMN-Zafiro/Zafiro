using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Zafiro.Core;

namespace Zafiro.Uwp.ObjEditor
{
    public class ObjectEditor : Control
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems", typeof(object), typeof(ObjectEditor),
            new PropertyMetadata(default, OnSelectedItemsChanged));

        public static readonly DependencyProperty PropertyItemsProperty = DependencyProperty.Register(
            "PropertyItems", typeof(IList<PropertyItem>), typeof(ObjectEditor),
            new PropertyMetadata(default(IList<PropertyItem>), OnProperyItemsChanged));

        public static readonly DependencyProperty EditorsProperty = DependencyProperty.Register(
            "Editors", typeof(EditorCollection), typeof(ObjectEditor), new PropertyMetadata(new EditorCollection()));

        public static readonly DependencyProperty DefaultEditorTemplateProperty = DependencyProperty.Register(
            "DefaultEditorTemplate", typeof(DataTemplate), typeof(ObjectEditor),
            new PropertyMetadata(default(DataTemplate)));

        private CompositeDisposable disposables = new CompositeDisposable();

        public ObjectEditor()
        {
            DefaultStyleKey = typeof(ObjectEditor);
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

        private static void OnSelectedItemsChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var target = (ObjectEditor) dependencyObject;
            target.OnSelectedItemsChanged(dependencyPropertyChangedEventArgs.OldValue,
                dependencyPropertyChangedEventArgs.NewValue);
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
                    System.Reactive.Disposables.DisposableMixins.DisposeWith(Observable
                        .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                            h => col.CollectionChanged += h, h => col.CollectionChanged -= h)
                        .Subscribe(x => OnCollectionChanged()), disposables);
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
                : EnumerableExtensions.GetCommon(allProperties, equalityComparer);

            return commonProperties.Select(o => new PropertyItem(o, targets))
                .OrderBy(item => item.PropertyName)
                .ToList();
        }

        private void OnCollectionChanged()
        {
            UpdatePropertyItems((IList<object>) SelectedItems);
        }

        private static void OnProperyItemsChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            var target = (ObjectEditor) dependencyObject;
            target.OnPropertyItemsChanged((IList<PropertyItem>) args.OldValue, (IList<PropertyItem>) args.NewValue);
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