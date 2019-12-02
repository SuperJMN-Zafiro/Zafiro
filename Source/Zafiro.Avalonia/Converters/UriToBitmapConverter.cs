using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Serilog;

namespace Zafiro.Avalonia.Converters
{
    public class UriToBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Uri uri)
            {
                var finalUri = GetAssetUri(uri);

                if (finalUri != null)
                {
                    try
                    {
                        var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                        using (var stream = assets.Open(finalUri))
                        {
                            if (stream != null)
                            {
                                return new Bitmap(stream);
                            }
                        }
                    }
                    catch (FileNotFoundException ex)
                    {
                        Log.Warning(ex, "Cannot find {Uri}. Please, verify the Uri", finalUri);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to load Uri at {Uri}", finalUri);
                    }
                }
            }

            return AvaloniaProperty.UnsetValue;
        }

        private static Uri GetAssetUri(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
            {
                var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
                if (assemblyName == null)
                {
                    Log.Warning("Could not get the Entry Assembly to determine the base Uri for {Uri}", uri);
                    return null;
                }

                var baseUri = new Uri($"avares://{assemblyName}/");
                return new Uri(baseUri, uri);
            }

            return uri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}