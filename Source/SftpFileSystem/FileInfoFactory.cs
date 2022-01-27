using System.IO.Abstractions;

namespace SftpFileSystem;

public class FileInfoFactory : IFileInfoFactory
{
    private readonly FileSystem fileSystem;

    public FileInfoFactory(FileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    public IFileInfo FromFileName(string fileName)
    {
        return new FileInfo(fileSystem, fileSystem.Client.Get(fileName));
    }
}