namespace Zafiro.DivineBytes;

public interface IData
{
    IObservable<byte[]> Bytes { get; }
}