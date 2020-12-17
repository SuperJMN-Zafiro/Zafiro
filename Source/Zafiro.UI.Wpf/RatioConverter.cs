using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Zafiro.UI.Wpf
{
    [ValueConversion(typeof(string), typeof(string))]
    public class RatioConverter : MarkupExtension, IValueConverter
    {
        private static RatioConverter instance;

        public RatioConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        { 
            double size = System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
            return size.ToString("G0", CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return instance ?? (instance = new RatioConverter());
        }
    }
}