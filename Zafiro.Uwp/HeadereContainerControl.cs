using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Zafiro.Uwp.Controls
{
    public sealed class HeadereContainerControl : ContentControl
    {
        public HeadereContainerControl()
        {
            this.DefaultStyleKey = typeof(HeadereContainerControl);
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header", typeof(object), typeof(HeadereContainerControl), new PropertyMetadata(default(object)));

        public object Header
        {
            get { return (object) GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}
