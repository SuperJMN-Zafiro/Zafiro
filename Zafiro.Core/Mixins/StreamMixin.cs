using System.IO;
using System.Threading.Tasks;

namespace Zafiro.Core.Mixins
{
    public static class StreamMixin
    {
        public static Task<string> ReadToEnd(this Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEndAsync();
            }
        }

        public static async Task<byte[]> ReadBytes(this Stream stream)
        {
            int read;
            var buffer = new byte[stream.Length];
            int receivedBytes = 0;

            while ((read = await stream.ReadAsync(buffer, receivedBytes, buffer.Length)) < receivedBytes)
            {
                receivedBytes += read;
            }
            
            return buffer;
        }
    }
}