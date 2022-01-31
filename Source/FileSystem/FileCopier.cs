using System.IO.Abstractions;

namespace FileSystem;

public class BulkCopier
{
    private readonly FileSystemComparer fileSystemComparer;
    private readonly IFileSystemPathTranslator pathTranslator;

    public BulkCopier(FileSystemComparer systemComparer, IFileSystemPathTranslator pathTranslator)
    {
        fileSystemComparer = systemComparer;
        this.pathTranslator = pathTranslator;
    }

    public async Task Copy(IDirectoryInfo a, IDirectoryInfo b)
    {
        var diffs = await fileSystemComparer.Diff(a, b);
        foreach (var diff in diffs)
            switch (diff.Status)
            {
                case FileDiffStatus.RightOnly:
                    diff.Right.Delete();
                    break;
                case FileDiffStatus.Both:
                    await CopyFile(diff.Left, diff.Right);
                    break;
                case FileDiffStatus.LeftOnly:
                    var path = pathTranslator.Translate(diff.Left, a, b);
                    var toCreate = b.FileSystem.FileInfo.FromFileName(path);
                    toCreate.Directory.Create();
                    await CopyFile(diff.Left, toCreate);
                    break;
            }
    }

    private static async Task CopyFile(IFileInfo origin, IFileInfo destination)
    {
        await using var destStream = destination.Create();
        await origin.OpenRead().CopyToAsync(destStream);
    }
}

public class CachingFileSystem : IFileSystem
{
    private readonly IFileSystem fileSystemImplementation;

    public CachingFileSystem(IFileSystem fileSystemImplementation)
    {
        this.fileSystemImplementation = fileSystemImplementation;
        FileStream = new CachingFileStreamFactory(fileSystemImplementation.FileStream);
    }

    public IFile File => fileSystemImplementation.File;

    public IDirectory Directory => fileSystemImplementation.Directory;

    public IFileInfoFactory FileInfo => fileSystemImplementation.FileInfo;

    public IFileStreamFactory FileStream { get; }

    public IPath Path => fileSystemImplementation.Path;

    public IDirectoryInfoFactory DirectoryInfo => fileSystemImplementation.DirectoryInfo;

    public IDriveInfoFactory DriveInfo => fileSystemImplementation.DriveInfo;

    public IFileSystemWatcherFactory FileSystemWatcher => fileSystemImplementation.FileSystemWatcher;
}

public interface IStreamStore
{
    public Task Create(string path, Func<Stream> getContents);
    public Task Replace(string path, Func<Stream> getContents);
    public Task Delete(string path);
}

public class FileSystemStreamStore : IStreamStore
{
    private readonly IDirectoryInfo root;

    public FileSystemStreamStore(IDirectoryInfo root)
    {
        this.root = root;
    }

    public async Task Create(string path, Func<Stream> getContents)
    {
        await using var dest = root.FileSystem.FileInfo.FromFileName(path).OpenWrite();
        await getContents().CopyToAsync(dest);
    }

    public async Task Replace(string path, Func<Stream> getContents)
    {
        await using var dest = root.FileSystem.FileInfo.FromFileName(path).OpenWrite();
        await using var con = getContents();
        await con.CopyToAsync(dest);
    }

    public Task Delete(string path)
    {
        var file = root.FileSystem.FileInfo.FromFileName(path);
        file.Delete();
        return Task.CompletedTask;
    }
}