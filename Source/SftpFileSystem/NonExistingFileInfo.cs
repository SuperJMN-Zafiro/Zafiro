namespace SftpFileSystem;

public class NonExistingFileInfo : FileInfoBase
{
    public NonExistingFileInfo(string fileName, FileSystem fileSystem) : base(fileSystem)
    {
        this.FullName = fileName;
    }

    public override bool Exists => false;
    public override string FullName { get; }

    public override long Length => 0;

    public override void Delete()
    {
    }
}