using System.IO.Abstractions;
using CSharpFunctionalExtensions;
using Serilog;

namespace Zafiro.FileSystem;

public class ZafiroFileSystem : IZafiroFileSystem
{
    public Maybe<ILogger> Logger { get; }
    private readonly IFileSystem fileSystem;

    public ZafiroFileSystem(IFileSystem fileSystem, Maybe<ILogger> logger)
    {
        Logger = logger;
        this.fileSystem = fileSystem;
    }

    public Result<IZafiroFile> GetFile(ZafiroPath path)
    {
        return Result
            .Try(() => fileSystem.FileInfo.FromFileName(TransformPathToFileSystem(path)))
            .CheckIf(info => !info.Exists, info => Result.Try(() =>
            {
                info.Directory.Create();
                info.Create();
            }))
            .Map(r => (IZafiroFile)new ZafiroFile(r, this));
    }

    public Result<IZafiroDirectory> GetDirectory(ZafiroPath path)
    {
        return Result
            .Try(() => fileSystem.DirectoryInfo.FromDirectoryName(TransformPathToFileSystem(path)))
            .CheckIf(info => !info.Exists, info => Result.Try(info.Create))
            .Map(r => (IZafiroDirectory)new ZafiroDirectory(r, this));
    }

    private string TransformPathToFileSystem(ZafiroPath path)
    {
        return string.Join(fileSystem.Path.DirectorySeparatorChar, path.RouteFragments);
    }
}