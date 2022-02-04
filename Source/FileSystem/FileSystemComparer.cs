using System.IO.Abstractions;
using MoreLinq;
using MoreLinq.Extensions;

namespace FileSystem;

public class FileSystemComparer : IFileSystemComparer
{
    public Task<IEnumerable<FileDiff>> Diff(IDirectoryInfo origin, IDirectoryInfo destination)
    {
        var originFiles = GetFilesRecursively(origin).Select(f => new
        {
            Key = GetKey(origin, f),
            File = f
        });
        var destinationFiles = GetFilesRecursively(destination).Select(f => new
        {
            Key = GetKey(destination, f),
            File = f
        });

        var fileDiffs = FullJoinExtension.FullJoin(originFiles, destinationFiles,
            f => f.Key,
            left => new FileDiff(left.File, null),
            right => new FileDiff(null, right.File),
            (left, right) => new FileDiff(left.File, right.File));

        return Task.FromResult(fileDiffs);
    }

    private static string GetKey(IFileSystemInfo origin, IFileInfo f)
    {
        return origin.GetRelativePath(f.FullName);
    }

    private static IEnumerable<IFileInfo> GetFilesRecursively(IDirectoryInfo origin)
    {
        if (!origin.Exists)
        {
            return Enumerable.Empty<IFileInfo>();
        }

        return MoreEnumerable.TraverseBreadthFirst(origin, dir => dir.EnumerateDirectories())
            .SelectMany(r => r.GetFiles());
    }
}