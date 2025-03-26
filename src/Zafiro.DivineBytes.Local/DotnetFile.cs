using System.IO.Abstractions;
using CSharpFunctionalExtensions;
using Zafiro.Reactive;

namespace Zafiro.DivineBytes.Local;

public class DotnetFile : IFile
{
    public IFileInfo Info { get; }

    public DotnetFile(IFileInfo info)
    {
        Info = info;
        this.Source = ByteSource.FromStreamFactory(info.OpenRead, () => Task.FromResult<Maybe<long>>(info.Length));
    }

    public IByteSource Source { get; }

    public IObservable<byte[]> Bytes => Source.Bytes;
    public Task<Maybe<long>> GetLength() => Source.GetLength();

    public string Name => Info.Name;
    public IDisposable Subscribe(IObserver<byte[]> observer)
    {
        return Bytes.Subscribe(observer);
    }
}