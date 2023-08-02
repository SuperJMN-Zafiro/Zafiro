using MoreLinq;

namespace Zafiro.FileSystem;

public static class ZafiroFileSystemMixin
{
    public static IEnumerable<IZafiroFile> GetAllFiles(this IZafiroDirectory origin)
    {
        return MoreEnumerable.TraverseBreadthFirst(origin, dir => dir.Directories)
            .SelectMany(r => r.Files);
    }

    public static ZafiroPath MakeRelativeTo(this ZafiroPath path, ZafiroPath relativeTo)
    {
        var relativePathChunks =
            relativeTo.RouteFragments
                .ZipLongest(path.RouteFragments, (x, y) => (x, y))
                .SkipWhile(x => x.x == x.y)
                .Select(x => { return x.x is null ? new[] {x.y} : new[] {"..", x.y}; })
                .Transpose()
                .SelectMany(x => x)
                .Where(x => x is not default(string));

        return new ZafiroPath(relativePathChunks);
    }


    public static ZafiroPath Combine(this ZafiroPath self, ZafiroPath path)
    {
        return new ZafiroPath(self.RouteFragments.Concat(path.RouteFragments));
    }

    public static async Task<byte[]> ReadAllBytes(this IZafiroFile file)
    {
        using (var memoryStream = new MemoryStream())
        using (var sourceStream = await file.OpenRead())
        {
            await sourceStream.CopyToAsync(memoryStream).ConfigureAwait(false);
            return memoryStream.ToArray();
        }
    }
}