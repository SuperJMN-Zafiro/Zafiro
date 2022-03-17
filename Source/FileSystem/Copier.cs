using CSharpFunctionalExtensions;

namespace FileSystem;

public class Copier : ICopier
{
    private readonly IZafiroFileSystemComparer comparer;

    public Copier(IZafiroFileSystemComparer systemComparer)
    {
        comparer = systemComparer;
    }

    public async Task<Result> Copy(IZafiroDirectory source, IZafiroDirectory destination)
    {
        var diffs = await comparer.Diff(source, destination).ConfigureAwait(false);

        return await diffs
            .Select(diff => Sync(diff, source, destination))
            .CombineInOrder(";").ConfigureAwait(false);
    }

    private async Task<Result> Sync(ZafiroFileDiff diff, IZafiroDirectory source, IZafiroDirectory destination)
    {
        switch (diff.Status)
        {
            case FileDiffStatus.RightOnly:
                return DeleteNonexistent(diff.Right.Value);
            case FileDiffStatus.Both:
                return await CopyOverExisting(diff.Left.Value, diff.Right.Value);
            case FileDiffStatus.LeftOnly:
                return await CopyNonexistent(source, destination, diff.Left.Value);
        }

        return Result.Failure("Invalid status");
    }

    private static async Task<Result> CopyNonexistent(IZafiroDirectory sourceDirectory,
        IZafiroDirectory destinationDirectory, IZafiroFile sourceFile)
    {
        var destPath = destinationDirectory.Path.Combine(sourceFile.Path.MakeRelativeTo(sourceDirectory.Path));

        return await destinationDirectory.FileSystem
            .GetFile(destPath)
            .Bind(destFile => sourceFile.CopyTo(destFile)).ConfigureAwait(false);
    }

    private static Result DeleteNonexistent(IZafiroFile file)
    {
        var delete = file.Delete();
        return delete;
    }

    private static async Task<Result> CopyOverExisting(IZafiroFile left, IZafiroFile right)
    {
        var copyTo = await left.CopyTo(right).ConfigureAwait(false);
        return copyTo;
    }
}