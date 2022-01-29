namespace FileSystem;

public class SmartFileSystemUtils : FileSystemUtils
{
    private readonly Cache cache;

    private readonly Dictionary<string, IDictionary<string, byte[]>> hashes = new();

    public SmartFileSystemUtils(Cache cache)
    {
        this.cache = cache;
    }

    protected override async Task CopyStream(FileSystemPath destination, string newPath, Func<Stream> streamFactory)
    {
        if (await AlreadyCopied(newPath, streamFactory)) return;

        cache.Add(newPath, streamFactory);
        await base.CopyStream(destination, newPath, streamFactory);
    }

    private Task<bool> AlreadyCopied(string path, Func<Stream> streamFactory)
    {
        return cache.Contains(path, streamFactory);
    }
}