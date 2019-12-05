using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Metadata;

namespace Zafiro.Avalonia.Converters
{
    public class ValueConverterGroup : List<IValueConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, culture));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public abstract class SingletonConverterBase<TConverter> : ConverterBase where TConverter : new()
    {
        private static readonly Lazy<TConverter> InstanceConstructor = new Lazy<TConverter>((Func<TConverter>)(() => new TConverter()), LazyThreadSafetyMode.PublicationOnly);

        public static TConverter Instance
        {
            get
            {
                return InstanceConstructor.Value;
            }
        }
    }

    public abstract class ConverterBase : AvaloniaObject, IValueConverter
    {
        protected static readonly object UnsetValue = AvaloniaProperty.UnsetValue;

        public abstract object Convert(object value,
            Type targetType,
            object parameter,
            CultureInfo culture);

        public virtual object ConvertBack(object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException("Converter '" + GetType().Name + "' does not support backward conversion.");
        }
    }

    public class StringToObjectConverter : SingletonConverterBase<StringToObjectConverter>
    {
        [Content]
        public ResourceDictionary Items { get; set; }

        public override object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is string key && Items != null && ContainsKey(Items, key))
                return ((IDictionary<object, object>)Items)[key];
            return UnsetValue;
        }

        private static bool ContainsKey(ResourceDictionary dict, string key)
        {
            return ((IDictionary<object, object>)dict).ContainsKey((object)key);
        }
    }
}