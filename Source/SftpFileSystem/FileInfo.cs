using System.IO.Abstractions;
using System.IO.Enumeration;
using System.Security.AccessControl;
using Renci.SshNet.Sftp;

namespace SftpFileSystem;

public class FileInfo : IFileInfo
{
    private readonly SftpFile file;
    private readonly FileSystem fileSystem;

    public FileInfo(FileSystem fileSystem, SftpFile file)
    {
        this.fileSystem = fileSystem;
        this.file = file;
        IsReadOnly = !(file.OwnerCanWrite || file.GroupCanWrite || file.OthersCanWrite);
    }

    public void Delete()
    {
        fileSystem.File.Delete(file.FullName);
    }

    public void Refresh()
    {
        throw new NotImplementedException();
    }

    public IFileSystem FileSystem => fileSystem;
    public FileAttributes Attributes { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime CreationTimeUtc { get; set; }
    public bool Exists => true;
    public string Extension => fileSystem.Path.GetExtension(Name);
    public string FullName => file.FullName;
    public DateTime LastAccessTime { get; set; }
    public DateTime LastAccessTimeUtc { get; set; }
    public DateTime LastWriteTime { get; set; }
    public DateTime LastWriteTimeUtc { get; set; }
    public string LinkTarget { get; }
    public string Name => file.Name;

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
        throw new NotImplementedException();
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
        return fileSystem.File.OpenText(FullName);
    }

    public Stream OpenWrite()
    {
        throw new NotImplementedException();
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

    public IDirectoryInfo Directory => new DirectoryInfo(DirectoryName, fileSystem);
    public string DirectoryName => fileSystem.Path.GetDirectoryName(FullName);
    public bool IsReadOnly { get; set; }

    public long Length => file.Length;
}