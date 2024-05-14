using System;

namespace Zafiro.DataModel;

public class Data : IData
{
    public Data(IObservable<byte[]> bytes, long length)
    {
        Bytes = bytes;
        Length = length;
    }

    public IObservable<byte[]> Bytes { get; }
    public long Length { get; }
}