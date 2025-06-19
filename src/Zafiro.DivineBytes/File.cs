using CSharpFunctionalExtensions;

namespace Zafiro.DivineBytes;

public class File(string name, IByteSource source) : INamedByteSource
{
    public string Name => name;

    public IDisposable Subscribe(IObserver<byte[]> observer)
    {
        return source.Subscribe(observer);
    }

    public Task<Maybe<long>> GetLength()
    {
        return source.GetLength();
    }

    public IObservable<byte[]> Bytes => source.Bytes;
}