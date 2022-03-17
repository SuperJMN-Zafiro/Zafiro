using CSharpFunctionalExtensions;
using Serilog;

namespace FileSystem;

public class Copier : ICopier
{
    private readonly IZafiroFileSystemComparer comparer;

    public Copier(IZafiroFileSystemComparer systemComparer)
    {
        comparer = systemComparer;
    }

    public async Task<Result> Copy(IZafiroDirectory source, IZafiroDirectory destination, Maybe<ILogger> logger)
    {
        var diffs = await comparer.Diff(source, destination).ConfigureAwait(false);

        return await diffs
            .Select(diff => Sync(diff, source, destination, logger))
            .CombineInOrder(";").ConfigureAwait(false);
    }

    private async Task<Result> Sync(ZafiroFileDiff diff, IZafiroDirectory source, IZafiroDirectory destination,
        Maybe<ILogger> logger)
    {
        switch (diff.Status)
        {
            case FileDiffStatus.RightOnly:
                return DeleteNonexistent(diff.Right.Value, logger);
            case FileDiffStatus.Both:
                return await CopyOverExisting(diff.Left.Value, diff.Right.Value, logger);
            case FileDiffStatus.LeftOnly:
                return await CopyNonexistent(source, destination, diff.Left.Value, logger);
        }

        return Result.Failure("Invalid status");
    }

    private static async Task<Result> CopyNonexistent(IZafiroDirectory sourceDirectory,
        IZafiroDirectory destinationDirectory, IZafiroFile sourceFile, Maybe<ILogger> logger)
    {
        var destPath = destinationDirectory.Path.Combine(sourceFile.Path.MakeRelativeTo(sourceDirectory.Path));

        return await destinationDirectory.FileSystem
            .GetFile(destPath)
            .Bind(async destFile =>
            {
                logger.Execute(l => l.Information("Copying {Source} to {Destination}", sourceFile, destFile));
                return await sourceFile.CopyTo(destFile);
            }).ConfigureAwait(false);
    }

    private static Result DeleteNonexistent(IZafiroFile file, Maybe<ILogger> logger)
    {
        logger.Execute(l => l.Information("Deleting {File}", file));
        var delete = file.Delete();
        return delete;
    }

    private static async Task<Result> CopyOverExisting(IZafiroFile left, IZafiroFile right, Maybe<ILogger> logger)
    {
        logger.Execute(l => l.Information("Copying {Source} to {Destination}", left, right));
        var copyTo = await left.CopyTo(right).ConfigureAwait(false);
        return copyTo;
    }
}