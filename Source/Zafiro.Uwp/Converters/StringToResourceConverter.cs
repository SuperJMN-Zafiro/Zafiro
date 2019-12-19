using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Zafiro.Uwp.Converters
{
    public class StringToResourceConverter : IValueConverter
    {
        public ResourceDictionary Resources { get; set; } = new ResourceDictionary();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string key)
            {
                return Resources[key];
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}