using System.IO.Abstractions;
using MoreLinq;
using MoreLinq.Extensions;

namespace FileSystem;

public class FileSystemComparer
{
    private readonly IFileComparer fileComparer;
    private readonly IFileSystemPathTranslator pathTranslator;

    public FileSystemComparer(IFileSystemPathTranslator pathTranslator, IFileComparer fileComparer)
    {
        this.pathTranslator = pathTranslator;
        this.fileComparer = fileComparer;
    }

    public async Task<IEnumerable<FileDiff>> Comparer(IDirectoryInfo origin, IDirectoryInfo destination)
    {
        var originFiles = GetFilesRecursively(origin);
        var destinationFiles = GetFilesRecursively(destination);

        return FullJoinExtension.FullJoin(originFiles, destinationFiles,
            f => pathTranslator.Translate(f, origin, destination),
            info => new FileDiff(info.FullName, FileDiffStatus.Deleted),
            info => new FileDiff(info.FullName, FileDiffStatus.Created),
            (info, fileInfo) => fileComparer.AreEqual(info, fileInfo)
                ? new FileDiff(info.FullName, FileDiffStatus.Unchanged)
                : new FileDiff(info.FullName, FileDiffStatus.Modified));
    }

    private static IEnumerable<IFileInfo> GetFilesRecursively(IDirectoryInfo origin)
    {
        return MoreEnumerable.TraverseBreadthFirst(origin, dir => dir.EnumerateDirectories())
            .SelectMany(r => r.GetFiles());
    }
}