using System;
using System.Reactive.Linq;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Zafiro.Uwp.Controls.Lightweight
{
    public class RTE : RichEditBox
    {
        public RTE()
        {
            Observable
                .FromEventPattern<RoutedEventHandler, RoutedEventArgs>(x => this.LostFocus += x, x => this.LostFocus -= x)
                .Select(ea =>
                {
                    Document.GetText(TextGetOptions.FormatRtf, out var t);
                    return t;
                })
                .DistinctUntilChanged()
                .Subscribe(s => Text = s);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(RTE), new PropertyMetadata(default(string), OnTextChanged));

        private static void OnTextChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            var rte = (RTE) target;
            var o = (string) args.OldValue;
            var n = (string) args.NewValue;

            rte.OnTextChanged(o, n);
        }

        private void OnTextChanged(string oldValue, string newValue)
        {
            Document.SetText(TextSetOptions.FormatRtf | TextSetOptions.CheckTextLimit | TextSetOptions.ApplyRtfDocumentDefaults, newValue);
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}