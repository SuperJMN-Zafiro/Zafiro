using CSharpFunctionalExtensions;

namespace Zafiro.DivineBytes;

public interface IByteSource : IObservable<byte[]>
{
    IObservable<byte[]> Bytes { get; }
    Task<Maybe<long>> GetLength();
}