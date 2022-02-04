using Renci.SshNet.Sftp;

namespace SftpFileSystem;

public class ExistingFileInfo : FileInfoBase
{
    private readonly SftpFile file;

    public ExistingFileInfo(FileSystem fileSystem, SftpFile file) : base(fileSystem)
    {
        this.file = file;
    }

    public override bool Exists => true;
    public override string FullName => file.FullName;
    public override long Length => file.Length;

    public override void Delete()
    {
        InnerFileSystem.File.Delete(FullName);
    }
}