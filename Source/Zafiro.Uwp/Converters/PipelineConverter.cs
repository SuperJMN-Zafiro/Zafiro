using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace Zafiro.Uwp.Converters
{
    public class PipelineConverter : List<IValueConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return this.Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, language));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}