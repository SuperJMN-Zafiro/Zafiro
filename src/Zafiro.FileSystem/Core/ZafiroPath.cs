using System.Diagnostics;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Core;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class ZafiroPath : ValueObject
{
    public const char ChunkSeparator = '/';

    public static readonly ZafiroPath Empty = new();

    public ZafiroPath(string path) : this(GetChunks(path))
    {
    }

    public ZafiroPath(IEnumerable<string> relativePathChunks)
    {
        RouteFragments = relativePathChunks;
    }

    private ZafiroPath() : this(Enumerable.Empty<string>())
    {
    }

    public IEnumerable<string> RouteFragments { get; }

    public string Path => string.Join(ChunkSeparator, RouteFragments);

    private string DebuggerDisplay => RouteFragments.Any() ? Path : "<root>";

    public static implicit operator ZafiroPath(string[] chunks)
    {
        return new ZafiroPath(chunks);
    }

    public static implicit operator ZafiroPath(string path)
    {
        if (path == "")
        {
            return Empty;
        }

        return new ZafiroPath(path);
    }

    public static implicit operator string(ZafiroPath path)
    {
        return path.ToString();
    }

    public static Result<ZafiroPath> Create(string path)
    {
        if (path.Trim() == "")
        {
            return Result.Failure<ZafiroPath>("Use ZafiroPath.Empty to create an empty path (usually root)");
        }

        if (GetChunks(path).Any(string.IsNullOrEmpty))
        {
            return Result.Failure<ZafiroPath>($"Invalid path {path}");
        }

        return new ZafiroPath(path);
    }

    public override string ToString()
    {
        return Path;
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Path;
    }

    private static IEnumerable<string> GetChunks(string path)
    {
        return path.Split(ChunkSeparator).ToArray();
    }
}