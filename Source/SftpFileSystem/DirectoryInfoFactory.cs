using System.IO.Abstractions;

namespace SftpFileSystem;

public class DirectoryInfoFactory : IDirectoryInfoFactory
{
    private readonly FileSystem fileSystem;

    public DirectoryInfoFactory(FileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    public IDirectoryInfo FromDirectoryName(string directoryName)
    {
        return new DirectoryInfo(directoryName, fileSystem);
    }
}