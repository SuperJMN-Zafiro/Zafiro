using System.IO.Abstractions;
using System.Security.AccessControl;
using Renci.SshNet;

namespace Zafiro.SftpFileSystem;

public class Directory : IDirectory
{
    private readonly FileSystem fileSystem;

    public Directory(FileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    private SftpClient Client => fileSystem.Client;

    public IDirectoryInfo CreateDirectory(string path)
    {
        if (Exists(path))
        {
            return new DirectoryInfo(path, fileSystem);
        }

        var parent = GetParent(path);
        if (parent != null && !parent.Exists)
        {
            parent.Create();
        }

        Client.CreateDirectory(path);

        return new DirectoryInfo(path, fileSystem);
    }

    public IDirectoryInfo CreateDirectory(string path, DirectorySecurity directorySecurity)
    {
        Client.CreateDirectory(path);
        return new DirectoryInfo(path, fileSystem);
    }

    public void Delete(string path)
    {
        var contents = Client.ListDirectory(path);
        foreach (var sftpFile in contents.Where(s => s.IsRegularFile))
        {
            sftpFile.Delete();
        }

        Client.DeleteDirectory(path);
    }

    public void Delete(string path, bool recursive)
    {
        if (recursive)
        {
            DeleteRecursive(path);
        }
        else
        {
            DeleteNonRecursive(path);
        }
    }

    private void DeleteRecursive(string path)
    {
        foreach (var file in Client.ListDirectory(path))
        {
            if (file.Name != "." && file.Name != "..")
            {
                if (file.IsDirectory)
                {
                    DeleteRecursive(file.FullName);
                }
                else
                {
                    Client.DeleteFile(file.FullName);
                }
            }
        }

        Client.DeleteDirectory(path);
    }

    private void DeleteNonRecursive(string path)
    {
        var contents = Client.ListDirectory(path);
        foreach (var sftpFile in contents.Where(s => s.IsRegularFile))
        {
            sftpFile.Delete();
        }

        Client.DeleteDirectory(path);
    }

    public bool Exists(string path)
    {
        return Client.Exists(path);
    }

    public DirectorySecurity GetAccessControl(string path)
    {
        throw new NotImplementedException();
    }

    public DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections)
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

    public string GetCurrentDirectory()
    {
        return Client.WorkingDirectory;
    }

    public string[] GetDirectories(string path)
    {
        return EnumerateDirectories(path).ToArray();
    }

    public string[] GetDirectories(string path, string searchPattern)
    {
        return EnumerateDirectories(path).ToArray();
    }

    public string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
    {
        return EnumerateDirectories(path).ToArray();
    }

    public string[] GetDirectories(string path, string searchPattern, EnumerationOptions enumerationOptions)
    {
        return EnumerateDirectories(path).ToArray();
    }

    public string GetDirectoryRoot(string path)
    {
        return fileSystem.Path.PathSeparator.ToString();
    }

    public string[] GetFiles(string path)
    {
        return EnumerateFiles(path).ToArray();
    }

    public string[] GetFiles(string path, string searchPattern)
    {
        return EnumerateFiles(path).ToArray();
    }

    public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
    {
        return EnumerateFiles(path).ToArray();
    }

    public string[] GetFiles(string path, string searchPattern, EnumerationOptions enumerationOptions)
    {
        return EnumerateFiles(path).ToArray();
    }

    public string[] GetFileSystemEntries(string path)
    {
        return EnumerateFileSystemEntries(path).ToArray();
    }

    public string[] GetFileSystemEntries(string path, string searchPattern)
    {
        return EnumerateFileSystemEntries(path).ToArray();
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

    public string[] GetLogicalDrives()
    {
        throw new NotImplementedException();
    }

    public IDirectoryInfo? GetParent(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        var directoryName = fileSystem.GetParentPath(path);

        if (string.IsNullOrEmpty(directoryName))
        {
            return null;
        }

        return new DirectoryInfo(directoryName, fileSystem);
    }

    public void Move(string sourceDirName, string destDirName)
    {
        throw new NotImplementedException();
    }

    public void SetAccessControl(string path, DirectorySecurity directorySecurity)
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

    public void SetCurrentDirectory(string path)
    {
        Client.ChangeDirectory(path);
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

    public IEnumerable<string> EnumerateDirectories(string path)
    {
        return fileSystem.GetDirectoryNames(path);
    }

    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
    {
        return fileSystem.GetDirectoryNames(path);
    }

    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
    {
        return fileSystem.GetDirectoryNames(path);
    }

    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern,
        EnumerationOptions enumerationOptions)
    {
        return fileSystem.GetDirectoryNames(path);
    }

    public IEnumerable<string> EnumerateFiles(string path)
    {
        return fileSystem.GetFilenames(path);
    }

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern)
    {
        return fileSystem.GetFilenames(path);
    }

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
    {
        return fileSystem.GetFilenames(path);
    }

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, EnumerationOptions enumerationOptions)
    {
        return fileSystem.GetFilenames(path);
    }

    public IEnumerable<string> EnumerateFileSystemEntries(string path)
    {
        return fileSystem.GetFilenames(path).Concat(fileSystem.GetDirectoryNames(path));
    }

    public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)
    {
        return fileSystem.GetFilenames(path).Concat(fileSystem.GetDirectoryNames(path));
    }

    public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
    {
        return fileSystem.GetFilenames(path).Concat(fileSystem.GetDirectoryNames(path));
    }

    public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern,
        EnumerationOptions enumerationOptions)
    {
        throw new NotImplementedException();
    }

    public IFileSystem FileSystem => fileSystem;
}