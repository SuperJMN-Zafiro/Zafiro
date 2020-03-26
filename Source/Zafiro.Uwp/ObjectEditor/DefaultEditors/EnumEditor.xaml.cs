using System;
using System.Collections;
using Windows.UI.Xaml;

namespace Zafiro.Uwp.Controls.ObjectEditor.DefaultEditors
{
    public sealed partial class EnumEditor
    {
        public EnumEditor()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(
            "Values", typeof(IEnumerable), typeof(EnumEditor), new PropertyMetadata(default(IEnumerable)));

        public IEnumerable Values
        {
            get { return (IEnumerable) GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(object), typeof(EnumEditor), new PropertyMetadata(default(object), OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           
            var target = (EnumEditor) d;
            target.Values = Enum.GetValues(target.Value.GetType());
            var newValue = e.NewValue;
            var oldValue = e.OldValue;
            target.OnValueChanged(oldValue, newValue);
        }

        private void OnValueChanged(object oldValue, object newValue)
        {
            if (newValue?.GetType() == oldValue?.GetType())
            {
                return;
            }

            Values = Enum.GetValues(Value.GetType());
        }

        public object Value
        {
            get { return (object) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }
}
