using System.IO.Abstractions;
using System.Security.AccessControl;

namespace Zafiro.SftpFileSystem;

public class DirectoryInfo : IDirectoryInfo
{
    private readonly FileSystem fileSystem;

    public DirectoryInfo(string directoryName, FileSystem fileSystem)
    {
        Name = directoryName;
        FullName = fileSystem.Path.GetFullPath(directoryName);
        this.fileSystem = fileSystem;
        Exists = fileSystem.Client.Exists(FullName);
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }

    public void Refresh()
    {
        throw new NotImplementedException();
    }

    public IFileSystem FileSystem => fileSystem;
    public FileAttributes Attributes { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime CreationTimeUtc { get; set; }
    public bool Exists { get; }
    public string Extension { get; }
    public string FullName { get; }

    public DateTime LastAccessTime { get; set; }
    public DateTime LastAccessTimeUtc { get; set; }
    public DateTime LastWriteTime { get; set; }
    public DateTime LastWriteTimeUtc { get; set; }
    public string LinkTarget { get; }
    public string Name { get; }

    public void Create()
    {
        fileSystem.Directory.CreateDirectory(FullName);
    }

    public void Create(DirectorySecurity directorySecurity)
    {
        fileSystem.Directory.CreateDirectory(FullName);
    }

    public IDirectoryInfo CreateSubdirectory(string path)
    {
        var subDir = fileSystem.Path.Combine(FullName, path);
        fileSystem.Directory.CreateDirectory(subDir);
        return new DirectoryInfo(subDir, fileSystem);
    }

    public void Delete(bool recursive)
    {
        fileSystem.Directory.Delete(FullName, recursive);
    }

    public IEnumerable<IDirectoryInfo> EnumerateDirectories()
    {
        return fileSystem.Directory.EnumerateDirectories(FullName).Select(s => new DirectoryInfo(s, fileSystem));
    }

    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, EnumerationOptions enumerationOptions)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IFileInfo> EnumerateFiles()
    {
        return GetAllFiles();
    }

    public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern)
    {
        return GetAll();
    }

    public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption)
    {
        return GetAll();
    }

    public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, EnumerationOptions enumerationOptions)
    {
        return GetAll();
    }

    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos()
    {
        return GetAll();
    }

    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern,
        EnumerationOptions enumerationOptions)
    {
        throw new NotImplementedException();
    }

    public DirectorySecurity GetAccessControl()
    {
        throw new NotImplementedException();
    }

    public DirectorySecurity GetAccessControl(AccessControlSections includeSections)
    {
        throw new NotImplementedException();
    }

    public IDirectoryInfo[] GetDirectories()
    {
        throw new NotImplementedException();
    }

    public IDirectoryInfo[] GetDirectories(string searchPattern)
    {
        throw new NotImplementedException();
    }

    public IDirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
    {
        throw new NotImplementedException();
    }

    public IDirectoryInfo[] GetDirectories(string searchPattern, EnumerationOptions enumerationOptions)
    {
        throw new NotImplementedException();
    }

    public IFileInfo[] GetFiles()
    {
        return GetAllFiles().ToArray();
    }

    public IFileInfo[] GetFiles(string searchPattern)
    {
        return GetAllFiles().ToArray();
    }

    public IFileInfo[] GetFiles(string searchPattern, SearchOption searchOption)
    {
        return GetAllFiles().ToArray();
    }

    public IFileInfo[] GetFiles(string searchPattern, EnumerationOptions enumerationOptions)
    {
        return GetAllFiles().ToArray();
    }

    public IFileSystemInfo[] GetFileSystemInfos()
    {
        return GetAllFiles().ToArray();
    }

    public IFileSystemInfo[] GetFileSystemInfos(string searchPattern)
    {
        return GetAllFiles().ToArray();
    }

    public IFileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption)
    {
        return GetAllFiles().ToArray();
    }

    public IFileSystemInfo[] GetFileSystemInfos(string searchPattern, EnumerationOptions enumerationOptions)
    {
        return GetAllFiles().ToArray();
    }

    public void MoveTo(string destDirName)
    {
        throw new NotImplementedException();
    }

    public void SetAccessControl(DirectorySecurity directorySecurity)
    {
        throw new NotImplementedException();
    }

    public IDirectoryInfo? Parent
    {
        get
        {
            var sshDirectoryInfo = FullName != string.Empty
                ? new DirectoryInfo(FileSystem.GetParentPath(FullName), fileSystem)
                : default;

            return sshDirectoryInfo;
        }
    }

    public IDirectoryInfo Root => new DirectoryInfo("/", fileSystem);

    private IEnumerable<ExistingFileInfo> GetAllFiles()
    {
        return GetAll();
    }

    private IEnumerable<ExistingFileInfo> GetAll()
    {
        var listDirectory = fileSystem.Client.ListDirectory(FullName);
        return listDirectory
            .Where(file => file.IsRegularFile)
            .Select(file => new ExistingFileInfo(fileSystem, file));
    }
}