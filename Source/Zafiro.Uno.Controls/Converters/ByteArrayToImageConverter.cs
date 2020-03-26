using System;
using System.IO;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Zafiro.Uno.Controls.Converters
{
    public class ByteArrayToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is byte[] bytes))
                return null;

            var image = new BitmapImage();

#if WINDOWS_UWP
            using (var stream = new InMemoryRandomAccessStream())
            {
              image.SetSource(stream);
              var s = stream.AsStreamForWrite();
              s.Write(bytes, 0, bytes.Length);
            }
#else
            Stream stream = new MemoryStream(bytes);
            image.SetSource(stream);
#endif
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}