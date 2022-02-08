using System.IO.Abstractions;
using System.Security.Cryptography;
using CSharpFunctionalExtensions;
using Serilog;

namespace FileSystem;

public class CachingFileManager : ICachingFileManager
{
    private static readonly SHA1 Hasher = SHA1.Create();

    private readonly string fileSystemName;
    private readonly IFileManager inner;

    public CachingFileManager(string fileSystemName, IFileManager inner, Dictionary<CacheKey, byte[]> hashes)
    {
        this.fileSystemName = fileSystemName;
        this.inner = inner;
        Cache = hashes;
    }

    public Dictionary<CacheKey, byte[]> Cache { get; }

    public async Task Copy(IFileInfo source, IFileInfo destination)
    {
        var hashedFile = new HashedFile(await GetHash(source.OpenRead), source);
        if (destination.Exists && AreEqual(hashedFile, destination))
        {
            Log.Verbose("{Source} has already been copied to {Destination} with the same contents. Skipping.",
                source.FullName, fileSystemName);
            return;
        }

        await inner.Copy(source, destination);
        Cache[ToKey(source, destination)] = hashedFile.Hash;
    }

    public void Delete(IFileInfo file)
    {
        var toRemove = Cache.Keys.Where(r => r.Destination == file.FullName);

        foreach (var actionKey in toRemove)
        {
            Cache.Remove(actionKey);
        }

        inner.Delete(file);
    }

    private static async Task<byte[]> GetHash(Func<Stream> getContents)
    {
        await using var stream = getContents();
        return await Hasher.ComputeHashAsync(stream);
    }

    private bool AreEqual(HashedFile source, IFileInfo destination)
    {
        var key = ToKey(source.File, destination);

        return Cache
            .TryFind(key)
            .Match(destHash => source.Hash.SequenceEqual(destHash), () => false);
    }

    private CacheKey ToKey(IFileInfo sourceFile, IFileInfo destination)
    {
        return new CacheKey
        {
            System = fileSystemName,
            Origin = sourceFile.FullName,
            Destination = destination.FullName
        };
    }

    private class HashedFile
    {
        public HashedFile(byte[] hash, IFileInfo file)
        {
            Hash = hash;
            File = file;
        }

        public byte[] Hash { get; }
        public IFileInfo File { get; }
    }
}