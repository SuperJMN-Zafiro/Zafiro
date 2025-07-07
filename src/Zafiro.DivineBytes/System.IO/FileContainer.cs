using System.IO.Abstractions;

namespace Zafiro.DivineBytes.System.IO;

internal class FileContainer(IFileInfo info) : INamedByteSource
{
    public IByteSource Source { get; } = ByteSource.FromStreamFactory(info.OpenRead);

    public string Name => info.Name;
    public IDisposable Subscribe(IObserver<byte[]> observer)
    {
        return Source.Subscribe(observer);
    }

    public IObservable<byte[]> Bytes => Source.Bytes;
}