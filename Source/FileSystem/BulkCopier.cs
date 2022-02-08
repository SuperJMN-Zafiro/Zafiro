using System.IO.Abstractions;
using CSharpFunctionalExtensions;

namespace FileSystem;

public class BulkCopier
{
    private readonly FileSystemComparer comparer;
    private readonly IFileManager fileManager;

    public BulkCopier(FileSystemComparer systemComparer,
        IFileManager fileManager)
    {
        comparer = systemComparer;
        this.fileManager = fileManager;
    }

    public async Task<Result> Copy(IDirectoryInfo source, IDirectoryInfo destination)
    {
        var diffs = await comparer.Diff(source, destination);

        return await diffs
            .Select(diff => Result.Try(() => Sync(diff, source, destination)))
            .CombineInOrder(";");
    }

    private async Task Sync(FileDiff diff, IFileSystemInfo source, IDirectoryInfo destination)
    {
        switch (diff.Status)
        {
            case FileDiffStatus.RightOnly:
                fileManager.Delete(diff.Right);
                break;
            case FileDiffStatus.Both:
                await fileManager.Copy(diff.Left, diff.Right);
                break;
            case FileDiffStatus.LeftOnly:
                var path = diff.Left.Translate(source, destination);
                var toCreate = destination.FileSystem.FileInfo.FromFileName(path);
                toCreate.Directory.Create();
                await fileManager.Copy(diff.Left, toCreate);
                break;
        }
    }
}