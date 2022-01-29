using System.IO.Abstractions;

namespace FileSystem;

public class SuperDirectory
{
    /// <summary>
    /// Represents a path from a given <see cref="IFileSystem"/>
    /// </summary>
    /// <param name="fileSystem">File system</param>
    /// <param name="path">Path in the file system</param>
    public SuperDirectory(IFileSystem fileSystem, string path)
    {
        FileSystem = fileSystem;
        Path = path;
    }

    public IFileSystem FileSystem { get; }
    public string Path { get; }

    public SuperDirectory WithPath(string newPath)
    {
        return new SuperDirectory(FileSystem, newPath);
    }

    public override string ToString()
    {
        return $"{nameof(Path)}: {Path}";
    }

    public IDirectoryInfo GetDirectory() => FileSystem.DirectoryInfo.FromDirectoryName(Path);
}