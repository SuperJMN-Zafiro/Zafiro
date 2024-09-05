using System.Reactive.Linq;
using Zafiro.FileSystem.Mutable;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Zafiro.FileSystem.Local;

public class WindowsRoot : IMutableDirectory
{
    public WindowsRoot(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }

    public IFileSystem FileSystem { get; }

    public string Name => "<root>";

    public bool IsHidden => false;

    public async Task<Result<IMutableDirectory>> CreateSubdirectory(string name)
    {
        return Result.Failure<IMutableDirectory>("Can't create subdirectories here");
    }

    public async Task<Result<IEnumerable<IMutableNode>>> GetChildren(CancellationToken cancellationToken = default)
    {
        return Result.Try(() =>
            FileSystem.DriveInfo.GetDrives().Select(driveInfo => driveInfo.RootDirectory)
                .Select(info => (IMutableNode)new Directory(info)));
    }

    public async Task<Result<bool>> HasFile(string name)
    {
        return false;
    }

    public Task<Result<bool>> HasSubdirectory(string name)
    {
        return GetChildren()
            .Bind(nodes => nodes.TryFirst(x => x.Name == name)
                .Match(_ => Result.Success(true), () => false));
    }

    public async Task<Result> DeleteFile(string name)
    {
        return Result.Failure<IMutableDirectory>("Can't delete files here");
    }

    public async Task<Result> DeleteSubdirectory(string name)
    {
        return Result.Failure<IMutableDirectory>("Can't delete subdirectories from root");
    }

    public IObservable<IMutableFile> FileCreated { get; } = Observable.Never<IMutableFile>();
    public IObservable<IMutableDirectory> DirectoryCreated { get; } = Observable.Never<IMutableDirectory>();
    public IObservable<string> FileDeleted { get; } = Observable.Never<string>();
    public IObservable<string> DirectoryDeleted { get; } = Observable.Never<string>();

    public async Task<Result<IMutableFile>> CreateFile(string entryName)
    {
        return Result.Failure<IMutableFile>("Can't create files in root");
    }
}