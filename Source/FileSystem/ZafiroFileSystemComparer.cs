using CSharpFunctionalExtensions;
using MoreLinq.Extensions;

namespace FileSystem;

public class ZafiroFileSystemComparer : IZafiroFileSystemComparer
{
    public Task<IEnumerable<ZafiroFileDiff>> Diff(IZafiroDirectory origin, IZafiroDirectory destination)
    {
        var originFiles = origin.GetAllFiles().Select(f => new
        {
            Key = GetKey(origin, f),
            File = f
        }).ToList();
        var destinationFiles = destination.GetAllFiles().Select(f => new
        {
            Key = GetKey(destination, f),
            File = f
        }).ToList();

        var fileDiffs = originFiles.FullJoin(destinationFiles,
            f => f.Key,
            left => new ZafiroFileDiff(Maybe.From(left.File), Maybe<IZafiroFile>.None),
            right => new ZafiroFileDiff(Maybe<IZafiroFile>.None, Maybe.From(right.File)),
            (left, right) => new ZafiroFileDiff(Maybe.From(left.File), Maybe.From(right.File)));

        return Task.FromResult(fileDiffs);
    }

    private static string GetKey(IZafiroDirectory origin, IZafiroFile f)
    {
        return f.Path.MakeRelativeTo(origin.Path).ToString();
    }
}