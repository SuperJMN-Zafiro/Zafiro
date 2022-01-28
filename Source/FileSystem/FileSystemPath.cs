using System.IO.Abstractions;

namespace FileSystem;

public class FileSystemPath
{
    /// <summary>
    /// Represents a path from a given <see cref="IFileSystem"/>
    /// </summary>
    /// <param name="fileSystem">File system</param>
    /// <param name="path">Path in the file system</param>
    public FileSystemPath(IFileSystem fileSystem, string path)
    {
        FileSystem = fileSystem;
        Path = path;
    }

    public IFileSystem FileSystem { get; }
    public string Path { get; }

    public FileSystemPath WithPath(string newPath)
    {
        return new FileSystemPath(FileSystem, newPath);
    }

    public override string ToString()
    {
        return $"{nameof(Path)}: {Path}";
    }
}