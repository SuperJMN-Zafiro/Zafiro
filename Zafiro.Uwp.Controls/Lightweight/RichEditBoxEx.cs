using System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Zafiro.Uwp.Controls.Lightweight
{
    public class RichEditBoxEx : RichEditBox
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RichEditBoxEx), new PropertyMetadata(string.Empty, Callback));

        public RichEditBoxEx()
        {
            TextChanged += RichEditEx_TextChanged;            
        }

        public TextGetOptions TextGetOption { get; } = TextGetOptions.FormatRtf;
        public TextSetOptions TextSetOption { get; } = TextSetOptions.FormatRtf;

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private void RichEditEx_TextChanged(object sender, RoutedEventArgs e)
        {
            var t = GetText();
            if (!string.Equals(t, Text, StringComparison.Ordinal))
            {
                Text = t;
            }
        }

        private string GetText()
        {
            string t;
            Document.GetText(TextGetOption, out t);
            return t;
        }


        private void SetText(string text)
        {
            if (GetText() != text)
            {
                Document.SetText(TextSetOption, text);
            }
        }

        private static void Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var reb = (RichEditBoxEx) d;
            reb.SetText((string) e.NewValue);
        }
    }
}