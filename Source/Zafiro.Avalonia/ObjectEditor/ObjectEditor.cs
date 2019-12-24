using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Templates;
using ReactiveUI;
using Zafiro.Core.ObjectEditor;
using Zafiro.Core.ObjectEditor.TemplateMatchers;

namespace Zafiro.Avalonia.ObjectEditor
{
    public class ObjectEditor : TemplatedControl, IObjectEditor<Control, DataTemplate>
    {
        public static readonly AvaloniaProperty SelectedItemsProperty =
            AvaloniaProperty.Register<ObjectEditor, object>(
                "SelectedItems", null, false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty PropertyItemsProperty =
            AvaloniaProperty.Register<ObjectEditor, IList<PropertyItem>>(
                "PropertyItems", default, false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty DefaultEditorTemplateProperty =
            AvaloniaProperty.Register<ObjectEditor, DataTemplate>(
                "DefaultEditorTemplate", default, false, BindingMode.TwoWay);

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public ObjectEditor()
        {
            var objectEditorCore = new ObjectEditorCore<Control, DataTemplate>(this,
                (dataTemplate, propertyInfo, targets) =>
                {
                    var control = (Control) dataTemplate?.Build(null);
                    return new PropertyItem(control, propertyInfo, targets);
                }, () => DefaultEditorTemplate);

            DefaultEditorTemplateProperty.Changed.Subscribe(args =>
            {
                objectEditorCore.OnSelectedItemsChanged(SelectedItems);
            });

            SelectedItemsProperty.Changed
                .Subscribe(x => objectEditorCore.OnSelectedItemsChanged(x.NewValue))
                .DisposeWith(disposables);

            objectEditorCore
                .WhenAnyValue(x => x.PropertyItems)
                .Subscribe(items => PropertyItems = items?.Cast<PropertyItem>().ToList())
                .DisposeWith(disposables);
        }

        public IList<PropertyItem> PropertyItems
        {
            get => (IList<PropertyItem>) GetValue(PropertyItemsProperty);
            set => SetValue(PropertyItemsProperty, value);
        }

        public DataTemplate DefaultEditorTemplate
        {
            get => (DataTemplate) GetValue(DefaultEditorTemplateProperty);
            set => SetValue(DefaultEditorTemplateProperty, value);
        }

        public EditorDefinitionCollection EditorDefinitions { get; set; } = new EditorDefinitionCollection();
        public EditorCollection<DataTemplate> EditorsCore => new EditorCollection<DataTemplate>(EditorDefinitions.Select(definition => new Editor<DataTemplate>(definition.Template, new EditorKey(definition.Key.TargetType, definition.Key.Properties.ToList()))).ToList());

        public object SelectedItems
        {
            get => GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }
    }
}