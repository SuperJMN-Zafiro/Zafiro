using System.Reactive.Linq;
using System.Text;
using Zafiro.Reactive;

namespace Zafiro.DivineBytes;

public class Data(IObservable<byte[]> bytes) : IData
{
 

    public Data(IObservable<IEnumerable<byte>> bytes) : this(bytes.Select(x => x.ToArray()))
    {
    }

    public IObservable<byte[]> Bytes { get; } = bytes;


    public static IData From(byte[] bytes, int bufferSize = 4096)
    {
        return new Data(bytes.ToByteStream(bufferSize));
    }

    public static IData From(IObservable<IEnumerable<byte>> bytes)
    {
        return new Data(bytes);
    }

    public static IData From(IObservable<byte[]> bytes)
    {
        return new Data(bytes);
    }

    public static IData From(Func<Stream> streamFactory)
    {
        return new Data(Observable.Using(streamFactory, stream => stream.ToObservable()));
    }

    public static IData From(string str, Encoding? encoding, int bufferSize = 4096)
    {
        return new Data(str.ToByteStream(encoding, bufferSize));
    }

    public static IData From(Func<Task<Stream>> streamFactory)
    {
        return new Data(ObservableFactory.UsingAsync(streamFactory, stream => stream.ToObservable()));
    }
}