using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace Zafiro.Uno.Converters
{
    public class TypeToStringConverter : List<IValueConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.GetType().Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}