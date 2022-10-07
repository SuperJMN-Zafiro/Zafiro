using System.IO.Abstractions;

namespace Zafiro.SftpFileSystem;

public static class FileSystemMixin
{
    public static IEnumerable<string> GetChunks(this IFileSystem fileSystem, string path)
    {
        return path.Split(fileSystem.Path.PathSeparator);
    }

    public static string GetParentPath(this IFileSystem fileSystem, string path)
    {
        var chunks = fileSystem.GetChunks(path);
        var skipLast = chunks.SkipLast(1);
        return fileSystem.Path.Combine(skipLast.ToArray());
    }

    public static IEnumerable<IFileInfo> GetFiles(this FileSystem fileSystem, string path)
    {
        return fileSystem.Client.ListDirectory(path)
            .Where(r => !r.IsDirectory || r.IsRegularFile)
            .Select(file => new ExistingFileInfo(fileSystem, file));
    }

    public static IEnumerable<string> GetFilenames(this FileSystem fileSystem, string path)
    {
        return GetFiles(fileSystem, path).Select(r => r.FullName);
    }


    public static IEnumerable<IDirectoryInfo> GetDirectories(this FileSystem fileSystem, string path)
    {
        return fileSystem.Client.ListDirectory(path)
            .Where(r => r.IsDirectory && r.Name != ".." && r.Name != ".")
            .Select(file => new DirectoryInfo(file.FullName, fileSystem));
    }

    public static IEnumerable<string> GetDirectoryNames(this FileSystem fileSystem, string path)
    {
        return GetDirectories(fileSystem, path).Select(r => r.FullName);
    }
}