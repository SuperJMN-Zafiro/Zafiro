using System.IO.Abstractions;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

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
                    await CopyStream(destination, translatedToDestination, file.OpenRead).ConfigureAwait(false);
                    break;
                case IDirectoryInfo dir:
                    await CopyTo(origin.WithPath(dir.FullName), destination.WithPath(translatedToDestination)).ConfigureAwait(false);
                    break;
            }
        }
    }

    public static string GetRelativePath(this IFileSystemInfo origin, string path)
    {
        return origin.FileSystem.Path.GetRelativePath(origin.FullName, path);
    }

    public static async Task<Result> Copy(this ISyncer syncer, Result<IZafiroDirectory> origin,
        Result<IZafiroDirectory> dest)
    {
        var copy =
            from o in origin
            from d in dest
            select new {Origin = o, Destination = d};

        return await copy.Bind(arg => syncer.Sync(arg.Origin, arg.Destination)).ConfigureAwait(false);
    }

    private static async Task CopyStream(FileSystemPath destination, string newPath, Func<Stream> streamFactory)
    {
        var newFile = destination.FileSystem.FileInfo.FromFileName(newPath);
        newFile.Directory.Create();
        using (var source = streamFactory())
        using (var dest = newFile.Create())
        {
            await source.CopyToAsync(dest).ConfigureAwait(false);
        }
    }
}