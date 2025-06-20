using CSharpFunctionalExtensions;
using Zafiro.DataModel;
using Zafiro.DivineBytes;

namespace Zafiro.Deployment.New.Platforms.Linux.Adapters;

internal class DataAdapter : IByteSource
{
    private readonly IData data;

    public DataAdapter(IData data)
    {
        this.data = data;
    }

    public IDisposable Subscribe(IObserver<byte[]> observer)
    {
        return data.Bytes.Subscribe(observer);
    }

    public IObservable<byte[]> Bytes => data.Bytes;
    public async Task<Maybe<long>> GetLength()
    {
        return Maybe.From(data.Length);
    }
}