using System.IO.Abstractions;

namespace Zafiro.SftpFileSystem;

public class FileInfoFactory : IFileInfoFactory
{
    private readonly FileSystem fileSystem;

    public FileInfoFactory(FileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    public IFileInfo FromFileName(string fileName)
    {
        return fileSystem.File.Exists(fileName)
            ? new ExistingFileInfo(fileSystem, fileSystem.Client.Get(fileName))
            : new NonExistingFileInfo(fileName, fileSystem);
    }
}