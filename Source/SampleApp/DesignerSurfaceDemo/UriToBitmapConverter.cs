using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace SampleApp.DesignerSurfaceDemo
{
    public class UriToBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Uri uri)
            {
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                var baseUri = new Uri("avares://SampleApp/");
                return new Bitmap(assets.Open(new Uri(baseUri, uri)));
            }

            return AvaloniaProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}