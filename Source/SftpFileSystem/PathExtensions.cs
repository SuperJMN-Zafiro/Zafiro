using System.IO.Abstractions;

namespace Zafiro.SftpFileSystem;

public static class PathExtensions
{
    public static IEnumerable<string> GetChunks(this IPath path, string stringPath)
    {
        return stringPath.Split(path.PathSeparator);
    }

    public static string GetParentPath(this IPath path, string stringPath)
    {
        var chunks = path.GetChunks(stringPath);
        var skipLast = chunks.SkipLast(1);
        return path.Combine(skipLast.ToArray());
    }
}