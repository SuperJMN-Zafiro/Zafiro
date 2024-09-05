using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Core;

public class FileSystemRoot : IFileSystemRoot
{
    private readonly IObservableFileSystem fs;

    public FileSystemRoot(IObservableFileSystem fs)
    {
        this.fs = fs;
    }

    public IZafiroFile GetFile(ZafiroPath path)
    {
        return new ZafiroFile(path, this);
    }

    public IZafiroDirectory GetDirectory(ZafiroPath path)
    {
        return new ZafiroDirectory(path, this);
    }

    public Task<Result<IEnumerable<IZafiroFile>>> GetFiles(ZafiroPath path, CancellationToken ct = default)
    {
        return fs.GetFilePaths(path, ct).Map(paths => paths.Select(zafiroPath => (IZafiroFile)new ZafiroFile(zafiroPath, this)));
    }

    public Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories(ZafiroPath path, CancellationToken ct = default)
    {
        return fs.GetDirectoryPaths(path, ct).Map(paths => paths.Select(zafiroPath => (IZafiroDirectory)new ZafiroDirectory(zafiroPath, this)));
    }

    public Task<Result<Stream>> GetFileData(ZafiroPath path)
    {
        return fs.GetFileData(path);
    }

    public Task<Result> SetFileData(ZafiroPath path, Stream stream, CancellationToken ct = default)
    {
        return fs.SetFileData(path, stream, ct);
    }

    public Task<Result<bool>> ExistFile(ZafiroPath path)
    {
        return fs.ExistFile(path);
    }

    public Task<Result> DeleteFile(ZafiroPath path)
    {
        return fs.DeleteFile(path);
    }

    public Task<Result> DeleteDirectory(ZafiroPath path)
    {
        return fs.DeleteDirectory(path);
    }

    public Task<Result<bool>> ExistDirectory(ZafiroPath path)
    {
        return fs.ExistDirectory(path);
    }

    public Task<Result> CreateFile(ZafiroPath path)
    {
        return fs.CreateFile(path);
    }

    public IObservable<byte> GetFileContents(ZafiroPath path)
    {
        return fs.GetFileContents(path);
    }

    public Task<Result> SetFileContents(ZafiroPath path, IObservable<byte> bytes, CancellationToken cancellationToken)
    {
        return fs.SetFileContents(path, bytes, cancellationToken);
    }

    public Task<Result> CreateDirectory(ZafiroPath path)
    {
        return fs.CreateDirectory(path);
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

    public IObservable<FileSystemChange> Changed => fs.Changed;
}