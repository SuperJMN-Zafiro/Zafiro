using System;
using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace Zafiro.Avalonia.ObjectEditor.DefaultEditors
{
    public class EnumEditor : UserControl
    {
        public EnumEditor()
        {
            this.InitializeComponent();

            ValueProperty.Changed.Subscribe(v => OnValueChanged(v.OldValue, v.NewValue));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static readonly AvaloniaProperty ValueProperty = AvaloniaProperty.Register<EnumEditor, object>(
            "Value", null, false, BindingMode.TwoWay);

        public static readonly AvaloniaProperty ValuesProperty = AvaloniaProperty.Register<EnumEditor, object>(
            "Values", null, false, BindingMode.TwoWay);


        private void OnValueChanged(object oldValue, object newValue)
        {
            if (newValue == null)
            {
                return;
            }

            if (newValue.GetType() == oldValue?.GetType())
            {
                return;
            }

            Values = Enum.GetValues(newValue.GetType());
        }

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public IEnumerable Values
        {
            get { return (IEnumerable)GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }
    }
}
