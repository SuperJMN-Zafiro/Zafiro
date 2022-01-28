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
        return fileSystem.File.Exists(fileName)
            ? new ExistingFileInfo(fileSystem, fileSystem.Client.Get(fileName))
            : new NonFileInfo(fileName, fileSystem);
    }
}

public class NonFileInfo : FileInfoBase
{
    private readonly string fileName;

    public NonFileInfo(string fileName, FileSystem fileSystem) : base(fileSystem)
    {
        this.fileName = fileName;
    }

    public override string FullName => fileName;
    public override long Length => 0;
    public override void Delete()
    {
    }
}