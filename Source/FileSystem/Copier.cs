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
                return diff.Right.Match(file => file.Delete(), Result.Success);
            case FileDiffStatus.Both:
                var left = diff.Left.Value;
                var right = diff.Right.Value;
                return await left.CopyTo(right).ConfigureAwait(false);
            case FileDiffStatus.LeftOnly:
                var l = diff.Left.Value;
                return await destination.FileSystem
                    .GetFile(destination.Path.Combine(l.Path.MakeRelativeTo(source.Path)))
                    .Bind(r => l.CopyTo(r)).ConfigureAwait(false);
        }

        return Result.Failure("Invalid status");
    }
}