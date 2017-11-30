using System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Zafiro.Uwp.Controls.Lightweight
{
    public class RichEditBoxEx : RichEditBox
    {
        public RichEditBoxEx()
        {
            this.TextChanged += RichEditEx_TextChanged;
        }

        private void RichEditEx_TextChanged(object sender, RoutedEventArgs e)
        {
            var t = GetText();
            if (!string.Equals(t, Text, StringComparison.Ordinal))
            {
                Text = t;
            }
        }
        public TextGetOptions TextGetOption { get; } = TextGetOptions.None;
        public TextSetOptions TextSetOption { get; } = TextSetOptions.FormatRtf;

        string GetText()
        {
            string t;
            this.Document.GetText(TextGetOption, out t);
            return t;
        }


        void SetText(string text)
        {
            if (GetText() != text)
            {
                this.Document.SetText(TextSetOption, text);
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        // Using a DependencyProperty as the backing store for TextProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(RichEditBoxEx), new PropertyMetadata(string.Empty, Callback));

        private static void Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var reb = (RichEditBoxEx)d;
            reb.SetText((string)e.NewValue);
        }
    }

}