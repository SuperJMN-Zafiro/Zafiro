using System.IO.Abstractions;
using CSharpFunctionalExtensions;

namespace FileSystem;

public class ZafiroFile : IZafiroFile
{
    private readonly IFileInfo fileInfo;

    public ZafiroFile(IFileInfo fileInfo, IZafiroFileSystem zafiroFileSystem)
    {
        this.fileInfo = fileInfo;
        FileSystem = zafiroFileSystem;
    }

    public ZafiroPath Path => new(fileInfo.FullName.Split(fileInfo.FileSystem.Path.DirectorySeparatorChar));

    public async Task<Result> CopyTo(IZafiroFile destination)
    {
        return await Result.Try(async () =>
        {
            await using var originStream = fileInfo.OpenRead();
            await using var destinationStream = destination.OpenWrite();
            await originStream.CopyToAsync(destinationStream).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public Stream OpenWrite()
    {
        return fileInfo.OpenWrite();
    }

    public Stream OpenRead()
    {
        return fileInfo.OpenRead();
    }

    public IZafiroFileSystem FileSystem { get; }

    public Result Delete()
    {
        return Result
            .Try(() => fileInfo.Delete())
            .OnSuccessTry(() => FileSystem.Logger.Execute(l => l.Verbose("Deleted '{Me}'", this)));
    }

    public override string ToString()
    {
        return Path.ToString();
    }
}