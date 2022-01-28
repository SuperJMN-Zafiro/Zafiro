using System.IO.Abstractions;

namespace FileSystem;

public static class FileSystemMixin
{
    /// <summary>
    /// Copies a directory from a <see cref="IFileSystem"/> to another
    /// </summary>
    /// <param name="origin">Target to copy from</param>
    /// <param name="destination">Target to copy to</param>
    /// <returns></returns>
    public static async Task CopyTo(this FileSystemPath origin, FileSystemPath destination)
    {
        var parent = origin.FileSystem.DirectoryInfo.FromDirectoryName(origin.Path);
        foreach (var item in parent.GetFileSystemInfos())
        {
            var newPath = MapPath(origin, destination, item.FullName);

            switch (item)
            {
                case IFileInfo file:
                    await CopyFile(destination, newPath, file);
                    break;
                case IDirectoryInfo dir:
                    await CopyTo(origin.From(dir.FullName), destination.From(newPath));
                    break;
            }
        }
    }

    private static async Task CopyFile(FileSystemPath destination, string newPath, IFileInfo file)
    {
        var newFile = destination.FileSystem.FileInfo.FromFileName(newPath);
        newFile.Directory.Create();
        await file.OpenRead().CopyToAsync(newFile.Create());
    }

    private static string MapPath(FileSystemPath origin, FileSystemPath destination, string path)
    {
        var relativeTo = origin.FileSystem.DirectoryInfo.FromDirectoryName(origin.Path).FullName;
        var relative = origin.FileSystem.Path.GetRelativePath(relativeTo, path);
        var subDir = destination.FileSystem.DirectoryInfo.FromDirectoryName(destination.Path);
        var newPath = destination.FileSystem.Path.Combine(subDir.FullName, relative);
        return newPath;
    }
}