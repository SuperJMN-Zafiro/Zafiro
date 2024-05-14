using System;
using System.Reactive.Linq;

namespace Zafiro.DataModel;

public class ByteArrayData : IData
{
    public ByteArrayData(byte[] content)
    {
        Bytes = Observable.Return(content);
        Length = content.Length;
    }

    public IObservable<byte[]> Bytes { get; }

    public long Length { get; }
}