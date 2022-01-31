using System.IO.Abstractions;

namespace FileSystem;

public class BulkCopier
{
    private readonly Func<DiffContext, IFileManager> fileManagerFactory;
    private readonly FileSystemComparer fileSystemComparer;
    private readonly IFileSystemPathTranslator pathTranslator;

    public BulkCopier(FileSystemComparer systemComparer, IFileSystemPathTranslator pathTranslator,
        Func<DiffContext, IFileManager> fileManagerFactory)
    {
        fileSystemComparer = systemComparer;
        this.pathTranslator = pathTranslator;
        this.fileManagerFactory = fileManagerFactory;
    }

    public async Task Copy(IDirectoryInfo a, IDirectoryInfo b)
    {
        var diffs = await fileSystemComparer.Diff(a, b);
        var fileManager = fileManagerFactory(new DiffContext(a, b));
        foreach (var diff in diffs)
            switch (diff.Status)
            {
                case FileDiffStatus.RightOnly:
                    fileManager.Delete(diff.Right);
                    break;
                case FileDiffStatus.Both:
                    await fileManager.Copy(diff.Left, diff.Right);
                    break;
                case FileDiffStatus.LeftOnly:
                    var path = pathTranslator.Translate(diff.Left, a, b);
                    var toCreate = b.FileSystem.FileInfo.FromFileName(path);
                    toCreate.Directory.Create();
                    await fileManager.Copy(diff.Left, toCreate);
                    break;
            }
    }

    private static async Task CopyFile(IFileInfo origin, IFileInfo destination)
    {
        await using var destStream = destination.Create();
        await origin.OpenRead().CopyToAsync(destStream);
    }
}

public interface IFileManager
{
    Task Copy(IFileInfo source, IFileInfo destination);
    void Delete(IFileInfo file);
}