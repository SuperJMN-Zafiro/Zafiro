using System.Reactive.Concurrency;
using CSharpFunctionalExtensions;
using Zafiro.DataModel;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Zafiro.Reactive;

namespace Zafiro.FileSystem.SeaweedFS;

public class File(ZafiroPath path, ISeaweedFS seaweedFS) : IMutableFile
{
    public ZafiroPath Path { get; } = path;
    public ISeaweedFS SeaweedFS { get; } = seaweedFS;

    public string Name => Path.Name();
    public bool IsHidden => false;

    public Task<Result> SetContents(IData data, IScheduler? scheduler = null, CancellationToken cancellationToken = default)
    {
        return SeaweedFS.Upload(Path, data.Bytes.ToStream(), cancellationToken);
    }

    public Task<Result<IData>> GetContents()
    {
        return SeaweedFS.GetFileMetadata(Path).Bind(GetData);
    }

    private Result<IData> GetData(FileMetadata metadata)
    {
        return Data.FromStream(() => SeaweedFS.GetFileContents(Path), metadata.FileSize);
    }

    public override string ToString()
    {
        return Path;
    }

    public Task<Result<bool>> Exists()
    {
        var exists = SeaweedFS.GetFileMetadata(Path)
            .Match(
                _ => Result.Success(true),
                err =>
                {
                    if (err.Contains("404"))
                    {
                        return Result.Success(false);
                    }

                    return Result.Failure<bool>(err);
                });
        return exists;
    }
}