using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using MoreLinq;

namespace Zafiro.FileSystem.Core;

public class ObservableFileSystem : IObservableFileSystem
{
    private readonly Subject<FileSystemChange> changed = new();
    private readonly IZafiroFileSystem fs;

    public ObservableFileSystem(IZafiroFileSystem fs)
    {
        this.fs = fs;
    }

    public IObservable<FileSystemChange> Changed => changed.AsObservable();

    public Task<Result<bool>> ExistFile(ZafiroPath path)
    {
        return fs.ExistFile(path);
    }

    public Task<Result> DeleteFile(ZafiroPath path)
    {
        return fs.DeleteFile(path).Tap(() => changed.OnNext(new FileSystemChange(path, Change.FileDeleted)));
    }

    public Task<Result> DeleteDirectory(ZafiroPath path)
    {
        return fs.DeleteDirectory(path).Tap(() => changed.OnNext(new FileSystemChange(path, Change.DirectoryDeleted)));
    }

    public Task<Result<Stream>> GetFileData(ZafiroPath path)
    {
        return fs.GetFileData(path);
    }

    public Task<Result> SetFileData(ZafiroPath path, Stream stream, CancellationToken cancellationToken)
    {
        return fs
            .SetFileData(path, stream, cancellationToken)
            .Tap(() => NotifyFileCreate(path));
    }

    public Task<Result> CreateFile(ZafiroPath path)
    {
        return fs
            .CreateFile(path)
            .Tap(() => NotifyFileCreate(path));
    }

    public IObservable<byte> GetFileContents(ZafiroPath path)
    {
        return fs.GetFileContents(path);
    }

    public async Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes, CancellationToken cancellationToken)
    {
        var changes = await Notifications.BeforeFileCreate(fs, path).ConfigureAwait(false);

        return await fs
            .SetFileContents(path, bytes, cancellationToken)
            .Tap(() => changes.ForEach(r => changed.OnNext(r)))
            .ConfigureAwait(false);
    }

    public Task<Result> CreateDirectory(ZafiroPath path)
    {
        return fs.CreateDirectory(path).Tap(() => changed.OnNext(new FileSystemChange(path, Change.DirectoryCreated)));
    }

    public Task<Result<FileProperties>> GetFileProperties(ZafiroPath path)
    {
        return fs.GetFileProperties(path);
    }

    public Task<Result<IDictionary<HashMethod, byte[]>>> GetHashes(ZafiroPath path)
    {
        return fs.GetHashes(path);
    }

    public Task<Result<DirectoryProperties>> GetDirectoryProperties(ZafiroPath path)
    {
        return fs.GetDirectoryProperties(path);
    }

    public Task<Result<IEnumerable<ZafiroPath>>> GetFilePaths(ZafiroPath path, CancellationToken ct = default)
    {
        return fs.GetFilePaths(path, ct);
    }

    public Task<Result<IEnumerable<ZafiroPath>>> GetDirectoryPaths(ZafiroPath path, CancellationToken ct = default)
    {
        return fs.GetDirectoryPaths(path, ct);
    }

    public Task<Result<bool>> ExistDirectory(ZafiroPath path)
    {
        return fs.ExistDirectory(path);
    }

    public Task<Result<bool>> ExistsDirectory(ZafiroPath path)
    {
        return fs.ExistDirectory(path);
    }

    private void NotifyFileCreate(ZafiroPath path)
    {
        path
            .Parents()
            .Select(zafiroPath => new FileSystemChange(zafiroPath, Change.DirectoryCreated))
            .Append(new FileSystemChange(path, Change.FileCreated))
            .ForEach(changed.OnNext);
    }
}