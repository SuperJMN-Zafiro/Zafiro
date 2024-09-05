using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS;

public class FileSystem : IMutableFileSystem
{
    public FileSystem(ISeaweedFS seaweedFS)
    {
        SeaweedFS = seaweedFS;
    }

    public ISeaweedFS SeaweedFS { get; }

    public Task<Result<IMutableDirectory>> GetDirectory(ZafiroPath path)
    {
        return Directory.From(path, SeaweedFS).Map(IMutableDirectory (s) => s);
    }

    public ZafiroPath InitialPath => ZafiroPath.Empty;
}