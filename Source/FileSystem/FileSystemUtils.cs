using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace FileSystem;

public class FileSystemUtils
{
    /// <summary>
    /// Copies a directory from a <see cref="IFileSystem"/> to another
    /// </summary>
    /// <param name="origin">Target to copy from</param>
    /// <param name="destination">Target to copy to</param>
    /// <returns></returns>
    public virtual async Task CopyTo(FileSystemPath origin, FileSystemPath destination)
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

    protected virtual async Task CopyStream(FileSystemPath destination, string newPath, Func<Stream> streamFactory)
    {
        var newFile = destination.FileSystem.FileInfo.FromFileName(newPath);
        newFile.Directory.Create();
        await using var source = streamFactory();
        await using var dest = newFile.Create();
        await source.CopyToAsync(dest);
    }
}