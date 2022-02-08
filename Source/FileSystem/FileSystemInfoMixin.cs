using System.IO.Abstractions;

namespace FileSystem;

public static class FileSystemInfoMixin
{
    public static string Translate(this IFileSystemInfo fileSystemInfo, IFileSystemInfo origin,
        IDirectoryInfo destination)
    {
        var relativeToSource = fileSystemInfo.FileSystem.Path.GetRelativePath(origin.FullName, fileSystemInfo.FullName);
        var originPathParts = relativeToSource.Split(fileSystemInfo.FileSystem.Path.DirectorySeparatorChar);
        var relativePathInDestination =
            string.Join(destination.FileSystem.Path.DirectorySeparatorChar, originPathParts);
        var translated = destination.FileSystem.Path.Combine(destination.FullName,
            relativePathInDestination);

        return translated;
    }
}