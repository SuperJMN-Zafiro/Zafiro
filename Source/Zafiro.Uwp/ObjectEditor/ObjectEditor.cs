using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ReactiveUI;
using Zafiro.Core.ObjectEditor;
using Zafiro.Core.ObjectEditor.TemplateMatchers;

namespace Zafiro.Uwp.ObjectEditor
{
    public class ObjectEditor : Control, IObjectEditor<FrameworkElement, DataTemplate>
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems", typeof(object), typeof(ObjectEditor),
            new PropertyMetadata(default, OnSelectedItemsChanged));

        public static readonly DependencyProperty DefaultEditorTemplateProperty = DependencyProperty.Register(
            "DefaultEditorTemplate", typeof(DataTemplate), typeof(ObjectEditor),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty PropertyItemsProperty = DependencyProperty.Register(
            "PropertyItems", typeof(IList<PropertyItem>), typeof(ObjectEditor),
            new PropertyMetadata(default(IList<PropertyItem>)));

        private readonly ObjectEditorCore<FrameworkElement, DataTemplate> objectEditorCore;

        public ObjectEditor()
        {
            DefaultStyleKey = typeof(ObjectEditor);
            objectEditorCore = new ObjectEditorCore<FrameworkElement, DataTemplate>(this,
                (dataTemplate, propertyInfo, targets) => new PropertyItem((FrameworkElement) dataTemplate.LoadContent(), propertyInfo, targets), () => DefaultEditorTemplate);

            objectEditorCore
                .WhenAnyValue(x => x.PropertyItems)
                .Subscribe(items => PropertyItems = items?.Cast<PropertyItem>().ToList());
        }

        public IList<PropertyItem> PropertyItems
        {
            get => (IList<PropertyItem>) GetValue(PropertyItemsProperty);
            set => SetValue(PropertyItemsProperty, value);
        }

        public DataTemplate DefaultEditorTemplate { get; set; }

        public EditorCollection Editors { get; set; } = new EditorCollection();
        public EditorCollection<DataTemplate> EditorsCore => new EditorCollection<DataTemplate>(Editors.ToList());

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ObjectEditor) d;
            target.objectEditorCore.OnSelectedItemsChanged(e.NewValue);
        }

        public object SelectedItems
        {
            get => GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }
    }
}