using System.IO.Abstractions;
using System.Security.AccessControl;
using System.Text;
using Renci.SshNet;

namespace SftpFileSystem;

public class File : IFile
{
    private readonly FileSystem fileSystem;

    public File(FileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    private SftpClient Client => fileSystem.Client;

    public Task AppendAllLinesAsync(string path, IEnumerable<string> contents,
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task AppendAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding,
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task AppendAllTextAsync(string path, string contents, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task AppendAllTextAsync(string path, string contents, Encoding encoding,
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task<string[]> ReadAllLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task<string> ReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task WriteAllLinesAsync(string path, IEnumerable<string> contents,
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task WriteAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding,
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task WriteAllLinesAsync(string path, string[] contents, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task WriteAllLinesAsync(string path, string[] contents, Encoding encoding,
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task WriteAllTextAsync(string path, string contents, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task WriteAllTextAsync(string path, string contents, Encoding encoding,
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public void AppendAllLines(string path, IEnumerable<string> contents)
    {
        Client.AppendAllLines(path, contents);
    }

    public void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)
    {
        Client.AppendAllLines(path, contents, encoding);
    }

    public void AppendAllText(string path, string contents)
    {
        Client.AppendAllText(path, contents);
    }

    public void AppendAllText(string path, string contents, Encoding encoding)
    {
        Client.AppendAllText(path, contents, encoding);
    }

    public StreamWriter AppendText(string path)
    {
        return Client.AppendText(path);
    }

    public void Copy(string sourceFileName, string destFileName)
    {
        using var source = fileSystem.Client.OpenRead(sourceFileName);
        using var destination = fileSystem.Client.OpenWrite(sourceFileName);
        source.CopyTo(destination);
    }

    public void Copy(string sourceFileName, string destFileName, bool overwrite)
    {
        Copy(sourceFileName, destFileName);
    }

    public Stream Create(string path)
    {
        return Client.Create(path);
    }

    public Stream Create(string path, int bufferSize)
    {
        return Client.Create(path, bufferSize);
    }

    public Stream Create(string path, int bufferSize, FileOptions options)
    {
        return Client.Create(path, bufferSize);
    }

    public StreamWriter CreateText(string path)
    {
        return Client.CreateText(path);
    }

    public void Decrypt(string path)
    {
        throw new NotImplementedException();
    }

    public void Delete(string path)
    {
        Client.Delete(path);
    }

    public void Encrypt(string path)
    {
        throw new NotImplementedException();
    }

    public bool Exists(string path)
    {
        return Client.Exists(path);
    }

    public FileSecurity GetAccessControl(string path)
    {
        throw new NotImplementedException();
    }

    public FileSecurity GetAccessControl(string path, AccessControlSections includeSections)
    {
        throw new NotImplementedException();
    }

    public FileAttributes GetAttributes(string path)
    {
        throw new NotImplementedException();
    }

    public DateTime GetCreationTime(string path)
    {
        throw new NotImplementedException();
    }

    public DateTime GetCreationTimeUtc(string path)
    {
        throw new NotImplementedException();
    }

    public DateTime GetLastAccessTime(string path)
    {
        throw new NotImplementedException();
    }

    public DateTime GetLastAccessTimeUtc(string path)
    {
        throw new NotImplementedException();
    }

    public DateTime GetLastWriteTime(string path)
    {
        throw new NotImplementedException();
    }

    public DateTime GetLastWriteTimeUtc(string path)
    {
        throw new NotImplementedException();
    }

    public void Move(string sourceFileName, string destFileName)
    {
        var client1 = fileSystem.Client;
        using var source = client1.OpenRead(sourceFileName);
        using var destination = client1.OpenWrite(sourceFileName);
        source.CopyTo(destination);
        Delete(sourceFileName);
    }

    public void Move(string sourceFileName, string destFileName, bool overwrite)
    {
        throw new NotImplementedException();
    }

    public Stream Open(string path, FileMode mode)
    {
        return Client.Open(path, mode);
    }

    public Stream Open(string path, FileMode mode, FileAccess access)
    {
        return Client.Open(path, mode, access);
    }

    public Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
    {
        return Open(path, mode, access);
    }

    public Stream OpenRead(string path)
    {
        return Client.OpenRead(path);
    }

    public StreamReader OpenText(string path)
    {
        return Client.OpenText(path);
    }

    public Stream OpenWrite(string path)
    {
        return Client.OpenWrite(path);
    }

    public byte[] ReadAllBytes(string path)
    {
        return Client.ReadAllBytes(path);
    }

    public string[] ReadAllLines(string path)
    {
        return Client.ReadAllLines(path);
    }

    public string[] ReadAllLines(string path, Encoding encoding)
    {
        return Client.ReadAllLines(path, encoding);
    }

    public string ReadAllText(string path)
    {
        return Client.ReadAllText(path);
    }

    public string ReadAllText(string path, Encoding encoding)
    {
        return Client.ReadAllText(path, encoding);
    }

    public IEnumerable<string> ReadLines(string path)
    {
        return Client.ReadLines(path);
    }

    public IEnumerable<string> ReadLines(string path, Encoding encoding)
    {
        return Client.ReadLines(path, encoding);
    }

    public void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName)
    {
        Client.RenameFile(sourceFileName, destinationFileName);
    }

    public void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName,
        bool ignoreMetadataErrors)
    {
        Client.RenameFile(sourceFileName, destinationFileName);
    }

    public void SetAccessControl(string path, FileSecurity fileSecurity)
    {
        throw new NotImplementedException();
    }

    public void SetAttributes(string path, FileAttributes fileAttributes)
    {
        throw new NotImplementedException();
    }

    public void SetCreationTime(string path, DateTime creationTime)
    {
        throw new NotImplementedException();
    }

    public void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
    {
        throw new NotImplementedException();
    }

    public void SetLastAccessTime(string path, DateTime lastAccessTime)
    {
        throw new NotImplementedException();
    }

    public void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
    {
        throw new NotImplementedException();
    }

    public void SetLastWriteTime(string path, DateTime lastWriteTime)
    {
        throw new NotImplementedException();
    }

    public void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
    {
        throw new NotImplementedException();
    }

    public void WriteAllBytes(string path, byte[] bytes)
    {
        Client.WriteAllBytes(path, bytes);
    }

    public void WriteAllLines(string path, IEnumerable<string> contents)
    {
        Client.WriteAllLines(path, contents);
    }

    public void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
    {
        Client.WriteAllLines(path, contents, encoding);
    }

    public void WriteAllLines(string path, string[] contents)
    {
        Client.WriteAllLines(path, contents);
    }

    public void WriteAllLines(string path, string[] contents, Encoding encoding)
    {
        Client.WriteAllLines(path, contents, encoding);
    }

    public void WriteAllText(string path, string contents)
    {
        Client.WriteAllText(path, contents);
    }

    public void WriteAllText(string path, string contents, Encoding encoding)
    {
        Client.WriteAllText(path, contents, encoding);
    }

    public IFileSystem FileSystem => fileSystem;
}