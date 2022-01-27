using System.IO.Abstractions;
using Renci.SshNet;

namespace SftpFileSystem;

public class FileSystem : IFileSystem
{
    private FileSystem(string host, int port, string username, string password)
    {
        Client = new SftpClient(host, port, username, password);
    }

    public SftpClient Client { get; }
    public IFile File => new File(this);
    public IDirectory Directory => new Directory(this);
    public IFileInfoFactory FileInfo { get; }
    public IFileStreamFactory FileStream { get; }
    public IPath Path => new Path(this);
    public IDirectoryInfoFactory DirectoryInfo => new DirectoryInfoFactory(this);
    public IDriveInfoFactory DriveInfo { get; }
    public IFileSystemWatcherFactory FileSystemWatcher { get; }

    public static FileSystem Create(string host, int port, string username, string password)
    {
        var fileSystem = new FileSystem(host, port, username, password);
        fileSystem.Client.Connect();
        return fileSystem;
    }
}