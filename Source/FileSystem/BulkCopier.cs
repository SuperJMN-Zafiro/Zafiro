using System.IO.Abstractions;
using CSharpFunctionalExtensions;

namespace FileSystem;

public class BulkCopier
{
    private readonly IFileManager fileManager;
    private readonly FileSystemComparer fileSystemComparer;
    private readonly IFileSystemPathTranslator pathTranslator;

    public BulkCopier(FileSystemComparer systemComparer, IFileSystemPathTranslator pathTranslator,
        IFileManager fileManager)
    {
        fileSystemComparer = systemComparer;
        this.pathTranslator = pathTranslator;
        this.fileManager = fileManager;
    }

    public async Task<Result> Copy(IDirectoryInfo a, IDirectoryInfo b)
    {
        ICollection<string> errors = new List<string>();
        var diffs = await fileSystemComparer.Diff(a, b);
        foreach (var diff in diffs)
        {
            try
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
                        var path = pathTranslator.Translate(diff.Left, a, b);
                        var toCreate = b.FileSystem.FileInfo.FromFileName(path);
                        toCreate.Directory.Create();
                        await fileManager.Copy(diff.Left, toCreate);
                        break;
                }
            }
            catch (Exception e)
            {
                errors.Add(e.Message);
            }
        }

        return Result.FailureIf(() => errors.Any(), string.Join(";", errors));
    }
}