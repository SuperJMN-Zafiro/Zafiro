using System.IO.Abstractions;
using System.Security.Cryptography;
using CSharpFunctionalExtensions;
using Serilog;

namespace FileSystem;

public class SmartFileManager : IFileManager
{
    private static readonly SHA1 Hasher = SHA1.Create();

    private readonly string fileSystemName;
    private readonly IFileManager inner;

    public SmartFileManager(string fileSystemName, IFileManager inner, Dictionary<Key, byte[]> hashes)
    {
        this.fileSystemName = fileSystemName;
        this.inner = inner;
        Hashes = hashes;
    }

    public Dictionary<Key, byte[]> Hashes { get; }

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
        Hashes[ToKey(source, destination)] = hashedFile.Hash;
    }

    public void Delete(IFileInfo file)
    {
        var toRemove = Hashes.Keys.Where(r => r.DestinationPath == file.FullName);

        foreach (var actionKey in toRemove)
        {
            Hashes.Remove(actionKey);
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

        return Hashes
            .TryFind(key)
            .Match(destHash => source.Hash.SequenceEqual(destHash), () => false);
    }

    private Key ToKey(IFileInfo sourceFile, IFileInfo destination)
    {
        return new Key
        {
            DestinationFileSystemName = fileSystemName,
            OriginPath = sourceFile.FullName,
            DestinationPath = destination.FullName
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

    public struct Key
    {
        public string OriginPath { get; set; }
        public string DestinationPath { get; set; }
        public string DestinationFileSystemName { get; set; }

        public bool Equals(Key other)
        {
            return OriginPath == other.OriginPath && DestinationPath == other.DestinationPath &&
                   DestinationFileSystemName == other.DestinationFileSystemName;
        }

        public override bool Equals(object? obj)
        {
            return obj is Key other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OriginPath, DestinationPath, DestinationFileSystemName);
        }
    }
}