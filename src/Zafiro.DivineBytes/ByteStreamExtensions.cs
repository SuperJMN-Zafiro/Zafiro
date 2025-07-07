using System.Reactive.Linq;
using System.Text;
using Zafiro.Mixins;

namespace Zafiro.DivineBytes;

public static class ByteStreamExtensions
{
    public static IObservable<IEnumerable<byte>> ToByteStream(this IEnumerable<byte> bytes, int bufferSize = 4096)
    {
        return bytes.ToObservable().Buffer(bufferSize);
    }

    public static IObservable<IEnumerable<byte>> ToByteStream(this string text, Encoding? encoding, int bufferSize = 4096)
    {
        return text.ToBytes(encoding ?? Encoding.UTF8).ToByteStream(bufferSize);
    }
}