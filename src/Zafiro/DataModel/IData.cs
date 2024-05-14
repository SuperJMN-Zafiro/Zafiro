using System;

namespace Zafiro.DataModel;

public interface IData
{
    public IObservable<byte[]> Bytes { get; }
    public long Length { get; }
}