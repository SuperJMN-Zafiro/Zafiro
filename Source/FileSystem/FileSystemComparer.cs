using System.IO.Abstractions;
using MoreLinq;
using MoreLinq.Extensions;

namespace FileSystem;

public class FileSystemComparer : IFileSystemComparer
{
    private readonly IFileComparer fileComparer;
    private readonly IFileSystemPathTranslator pathTranslator;

    public FileSystemComparer(IFileSystemPathTranslator pathTranslator, IFileComparer fileComparer)
    {
        this.pathTranslator = pathTranslator;
        this.fileComparer = fileComparer;
    }

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
            source => new FileDiff(source.File, FileDiffStatus.Deleted),
            dest => new FileDiff(GetSource(dest.File, origin, destination), FileDiffStatus.Created),
            (source, dest) => fileComparer.AreEqual(source.File, dest.File)
                ? new FileDiff(source.File, FileDiffStatus.Unchanged)
                : new FileDiff(source.File, FileDiffStatus.Modified));

        return Task.FromResult(fileDiffs);
    }

    private static string GetKey(IFileSystemInfo origin, IFileInfo f)
    {
        return origin.GetRelativePath(f.FullName);
    }

    private static IEnumerable<IFileInfo> GetFilesRecursively(IDirectoryInfo origin)
    {
        if (!origin.Exists) return Enumerable.Empty<IFileInfo>();

        return MoreEnumerable.TraverseBreadthFirst(origin, dir => dir.EnumerateDirectories())
            .SelectMany(r => r.GetFiles());
    }

    private IFileInfo GetSource(IFileInfo file, IDirectoryInfo origin, IDirectoryInfo destination)
    {
        return origin.FileSystem.FileInfo.FromFileName(pathTranslator.Translate(file, origin, destination));
    }
}