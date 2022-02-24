using CSharpFunctionalExtensions;

namespace FileSystem;

public sealed class ZafiroPath : ValueObject
{
    public const char ChuckSeparator = '/';

    public ZafiroPath(string path)
    {
        Path = path;
    }

    public ZafiroPath(params string[] routeFragments)
    {
        Path = string.Join(ChuckSeparator, routeFragments);
    }

    public ZafiroPath(IEnumerable<string> relativePathChunks) : this(relativePathChunks.ToArray())
    {
    }

    public IEnumerable<string> RouteFragments => Path.Split(ChuckSeparator);

    public static implicit operator ZafiroPath(string[] chunks)
    {
        return new ZafiroPath(chunks);
    }

    public static implicit operator ZafiroPath(string path)
    {
        return new ZafiroPath(path);
    }

    public static implicit operator string(ZafiroPath path)
    {
        return path.ToString();
    }

    public override string ToString()
    {
        return string.Join(ChuckSeparator, RouteFragments);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Path;
    }

    public string Path { get; }
}