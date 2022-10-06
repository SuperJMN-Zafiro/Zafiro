using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace FileSystem.Caching;

public class CachingZafiroFileSystem : IZafiroFileSystem
{
    private readonly HashSet<CopyOperationMetadata> hashSet;
    private readonly IZafiroFileSystem inner;

    public CachingZafiroFileSystem(IZafiroFileSystem inner)
    {
        this.inner = inner;
        Cache = new MemoryCache(new MemoryCacheOptions());
    }

    public Maybe<ILogger> Logger => inner.Logger;
    public MemoryCache Cache { get; }

    public Result<IZafiroFile> GetFile(ZafiroPath path)
    {
        return inner.GetFile(path)
            .Map(file => (IZafiroFile) new CachingZafiroFile(file, this));
    }

    public Result<IZafiroDirectory> GetDirectory(ZafiroPath path)
    {
        return inner.GetDirectory(path)
            .Map(dir => (IZafiroDirectory) new CachingZafiroDirectory(dir, this));
    }

    public void RemoveHash(ZafiroPath getHash)
    {
        hashSet.RemoveWhere(d => d.Source.Equals(getHash));
    }
}