namespace Zafiro.DivineBytes;

public interface IByteSource : IObservable<byte[]>
{
    IObservable<byte[]> Bytes { get; }
}