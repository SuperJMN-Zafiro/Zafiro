using System.IO.Abstractions;
using Zafiro.Reactive;

namespace Zafiro.DivineBytes.Local;

public class DotnetFile : IFile
{
    public IFileInfo Info { get; }

    public DotnetFile(IFileInfo info)
    {
        Info = info;
    }

    public IObservable<byte[]> Bytes => ReactiveData.ToObservable(() => new DisposeAwareStream(Info.OpenRead()));
    public string Name => Info.Name;
}