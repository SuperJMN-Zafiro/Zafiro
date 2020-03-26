using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ReactiveUI;
using Zafiro.Core.ObjectEditor;
using Zafiro.Core.ObjectEditor.TemplateMatchers;

namespace Zafiro.Uno.Controls.ObjectEditor
{
    public partial class ObjectEditor : Control, IObjectEditor<FrameworkElement, DataTemplate>
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems", typeof(object), typeof(ObjectEditor),
            new PropertyMetadata(default, (obj, _) => RefreshItems((ObjectEditor) obj)));

        public static readonly DependencyProperty DefaultEditorTemplateProperty = DependencyProperty.Register(
            "DefaultEditorTemplate", typeof(DataTemplate), typeof(ObjectEditor),
            new PropertyMetadata(null, (obj, __) => RefreshItems((ObjectEditor) obj)));

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

        public DataTemplate DefaultEditorTemplate
        {
            get => (DataTemplate) this.GetValue(DefaultEditorTemplateProperty);
            set => this.SetValue(DefaultEditorTemplateProperty, value);
        }

        public EditorCollection<DataTemplate> EditorsCore
        {
            get
            {
                return new EditorCollection<DataTemplate>(EditorDefinitions
                    .Select(d =>
                    {
                        if (d.DefinitionKey == null)
                        {
                            throw new InvalidOperationException($"The Key of an EditorDefinition cannot be null");
                        }

                        return new Editor<DataTemplate>(d.Template,
                            new EditorKey(d.DefinitionKey.TargetType, d.DefinitionKey.Properties.ToList()));
                    })
                    .ToList());
            }
        }

        private static void RefreshItems(ObjectEditor objectEditor)
        {
            objectEditor.objectEditorCore.OnSelectedItemsChanged(objectEditor.SelectedItems);
        }

        public object SelectedItems
        {
            get => GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public EditorDefinitionCollection EditorDefinitions { get; set; } = new EditorDefinitionCollection();
    }
}