using System.IO.Abstractions;

namespace FileSystem;

public static class FileSystemMixin
{
    /// <summary>
    ///     Copies a directory from a <see cref="IFileSystem" /> to another
    /// </summary>
    /// <param name="origin">Target to copy from</param>
    /// <param name="destination">Target to copy to</param>
    /// <returns></returns>
    public static async Task CopyTo(this FileSystemPath origin, FileSystemPath destination)
    {
        var parent = origin.FileSystem.DirectoryInfo.FromDirectoryName(origin.Path);
        foreach (var item in parent.GetFileSystemInfos())
        {
            var translatedToDestination = origin.Translate(item.FullName, destination);

            switch (item)
            {
                case IFileInfo file:
                    await CopyStream(destination, translatedToDestination, file.OpenRead);
                    break;
                case IDirectoryInfo dir:
                    await CopyTo(origin.WithPath(dir.FullName), destination.WithPath(translatedToDestination));
                    break;
            }
        }
    }

    public static string GetRelativePath(this IFileSystemInfo origin, string path)
    {
        return origin.FileSystem.Path.GetRelativePath(origin.FullName, path);
    }

    private static async Task CopyStream(FileSystemPath destination, string newPath, Func<Stream> streamFactory)
    {
        var newFile = destination.FileSystem.FileInfo.FromFileName(newPath);
        newFile.Directory.Create();
        await using var source = streamFactory();
        await using var dest = newFile.Create();
        await source.CopyToAsync(dest);
    }
}