using System;
using System.Linq;
using System.Reactive.Linq;

namespace Zafiro.DataModel;

public class CompositeData : IData
{
    public CompositeData(params IData[] byteProviders)
    {
        Bytes = byteProviders.Select(x => x.Bytes).Concat();
        Length = byteProviders.Sum(x => x.Length);
    }

    public IObservable<byte[]> Bytes { get; }
    public long Length { get; }
}