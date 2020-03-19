using System;
using System.Reactive;
using Windows.UI.Xaml.Data;

namespace Zafiro.UI.Infrastructure.Uno
{
    public class UnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Unit.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}