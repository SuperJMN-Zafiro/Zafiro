using System.IO.Abstractions;

namespace FileSystem;

public class BulkCopier
{
    private readonly FileSystemComparer fileSystemComparer;
    private readonly IFileSystemPathTranslator pathTranslator;

    public BulkCopier(FileSystemComparer systemComparer, IFileSystemPathTranslator pathTranslator)
    {
        fileSystemComparer = systemComparer;
        this.pathTranslator = pathTranslator;
    }

    public async Task Copy(IDirectoryInfo origin, IDirectoryInfo destination)
    {
        var diffs = await fileSystemComparer.Diff(origin, destination);
        foreach (var diff in diffs)
        {
            var translatedPath = pathTranslator.Translate(diff.Source, origin, destination);
            var destFile = destination.FileSystem.FileInfo.FromFileName(translatedPath);

            switch (diff.Status)
            {
                case FileDiffStatus.Created:
                    destFile.Delete();
                    break;
                case FileDiffStatus.Modified:
                    await diff.Source.OpenRead().CopyToAsync(destFile.OpenWrite());
                    break;
                case FileDiffStatus.Deleted:
                    destFile.Directory.Create();
                    await diff.Source.OpenRead().CopyToAsync(destFile.Create());
                    break;
            }
        }
    }
}