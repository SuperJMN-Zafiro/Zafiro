using System;
using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Zafiro.Uwp.Controls.ObjEditor.DefaultEditors
{
    public sealed partial class EnumEditor : UserControl
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
            if (e.NewValue?.GetType() == e.OldValue?.GetType())
            {
                return;
            }

            var target = (EnumEditor) d;
            target.Values = Enum.GetValues(target.Value.GetType());
        }

        public object Value
        {
            get { return (object) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }
}
