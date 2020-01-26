using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace SampleApp.Infrastructure
{
    public class SectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is NavigationViewItemInvokedEventArgs args)
            {
                if (args.InvokedItem is Section s)
                {
                    return s;
                }

                if (args.IsSettingsInvoked)
                {
                    return new Section("Settings", typeof(object));
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}