using System.IO.Abstractions;
using System.Security.AccessControl;

namespace SftpFileSystem;

public abstract class FileInfoBase : IFileInfo
{
    private readonly FileSystem innerFileSystem;

    protected FileInfoBase(FileSystem fileSystem)
    {
        this.innerFileSystem = fileSystem;
    }

    public IFileSystem FileSystem => InnerFileSystem;
    public FileAttributes Attributes { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime CreationTimeUtc { get; set; }
    public bool Exists => true;
    public string Extension => InnerFileSystem.Path.GetExtension(Name);
    public abstract string FullName { get; }
    public DateTime LastAccessTime { get; set; }
    public DateTime LastAccessTimeUtc { get; set; }
    public DateTime LastWriteTime { get; set; }
    public DateTime LastWriteTimeUtc { get; set; }
    public string LinkTarget { get; }
    public string Name => InnerFileSystem.Path.GetFileName(FullName);
    public IDirectoryInfo Directory => new DirectoryInfo(DirectoryName, InnerFileSystem);
    public string DirectoryName => InnerFileSystem.Path.GetDirectoryName(FullName);
    public bool IsReadOnly { get; set; }
    public abstract long Length { get; }

    protected FileSystem InnerFileSystem => innerFileSystem;

    public abstract void Delete();

    public void Refresh()
    {
        throw new NotImplementedException();
    }

    public StreamWriter AppendText()
    {
        throw new NotImplementedException();
    }

    public IFileInfo CopyTo(string destFileName)
    {
        throw new NotImplementedException();
    }

    public IFileInfo CopyTo(string destFileName, bool overwrite)
    {
        throw new NotImplementedException();
    }

    public Stream Create()
    {
        return InnerFileSystem.Client.Create(FullName);
    }

    public StreamWriter CreateText()
    {
        throw new NotImplementedException();
    }

    public void Decrypt()
    {
        throw new NotImplementedException();
    }

    public void Encrypt()
    {
        throw new NotImplementedException();
    }

    public FileSecurity GetAccessControl()
    {
        throw new NotImplementedException();
    }

    public FileSecurity GetAccessControl(AccessControlSections includeSections)
    {
        throw new NotImplementedException();
    }

    public void MoveTo(string destFileName)
    {
        throw new NotImplementedException();
    }

    public void MoveTo(string destFileName, bool overwrite)
    {
        throw new NotImplementedException();
    }

    public Stream Open(FileMode mode)
    {
        throw new NotImplementedException();
    }

    public Stream Open(FileMode mode, FileAccess access)
    {
        throw new NotImplementedException();
    }

    public Stream Open(FileMode mode, FileAccess access, FileShare share)
    {
        throw new NotImplementedException();
    }

    public Stream OpenRead()
    {
        throw new NotImplementedException();
    }

    public StreamReader OpenText()
    {
        return InnerFileSystem.File.OpenText(FullName);
    }

    public Stream OpenWrite()
    {
        return InnerFileSystem.File.OpenWrite(FullName);
    }

    public IFileInfo Replace(string destinationFileName, string destinationBackupFileName)
    {
        throw new NotImplementedException();
    }

    public IFileInfo Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
    {
        throw new NotImplementedException();
    }

    public void SetAccessControl(FileSecurity fileSecurity)
    {
        throw new NotImplementedException();
    }
}