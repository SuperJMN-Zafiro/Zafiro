namespace FileSystem;

public class SmartFileSystemUtils : FileSystemUtils
{

    public SmartFileSystemUtils(Cache cache)
    {
        this.cache = cache;
    }

    private readonly Dictionary<string, IDictionary<string, byte[]>> hashes = new();
    private readonly Cache cache;

    protected override async Task CopyStream(FileSystemPath destination, string newPath, Func<Stream> streamFactory)
    {
        if (AlreadyCopied(newPath))
        {
            return;
        }

        cache.Add(newPath, streamFactory);
        await base.CopyStream(destination, newPath, streamFactory);
    }

    private bool AlreadyCopied(string path)
    {
        return cache.Contains(path);
    }
}