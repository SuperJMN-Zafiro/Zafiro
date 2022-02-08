using System.IO.Abstractions;
using CSharpFunctionalExtensions;
using Renci.SshNet;

namespace SftpFileSystem;

public class FileSystem : IFileSystem, IDisposable
{
    private FileSystem(string host, int port, Credentials credentials)
    {
        Client = new SftpClient(host, port, credentials.Username, credentials.Password);
    }

    public SftpClient Client { get; }

    public void Dispose()
    {
        Client.Dispose();
    }

    public IFile File => new File(this);
    public IDirectory Directory => new Directory(this);
    public IFileInfoFactory FileInfo => new FileInfoFactory(this);
    public IFileStreamFactory FileStream { get; }
    public IPath Path => new Path(this);
    public IDirectoryInfoFactory DirectoryInfo => new DirectoryInfoFactory(this);
    public IDriveInfoFactory DriveInfo { get; }
    public IFileSystemWatcherFactory FileSystemWatcher { get; }

    public static Result<FileSystem> Connect(string host, int port,
        Credentials credentials)
    {
        return Result.Try(() =>
        {
            var fileSystem = new FileSystem(host, port, credentials);
            fileSystem.Client.Connect();
            return fileSystem;
        });
    }
}