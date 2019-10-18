using System.IO;
using System.Threading.Tasks;

namespace Zafiro.Core
{
    public static class StreamExtensions
    {
        public static Task<string> ReadToEnd(this Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEndAsync();
            }
        }
    }
}