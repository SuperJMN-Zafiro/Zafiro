using System.IO.Abstractions;
using System.Security.Cryptography;
using CSharpFunctionalExtensions;
using Serilog;

namespace FileSystem;

public class SmartFileManager : FileManager
{
    private static readonly SHA1 Hasher = SHA1.Create();
    private readonly string destinationFileSystemName;

    public SmartFileManager(string destinationFileSystemName)
    {
        this.destinationFileSystemName = destinationFileSystemName;
    }

    public Dictionary<Key, byte[]> Hashes { get; } = new();

    public override async Task Copy(IFileInfo source, IFileInfo destination)
    {
        var hashedFile = new HashedFile(await GetHash(source.OpenRead), source);
        if (AreEqual(hashedFile, destination))
        {
            Log.Verbose("{Source} has already been copied to {Destination} with the same contents. Skipping.",
                source.FullName, destinationFileSystemName);
            return;
        }

        await base.Copy(source, destination);
        Hashes[ToKey(source, destination)] = hashedFile.Hash;
    }

    public override void Delete(IFileInfo file)
    {
        var toRemove = Hashes.Keys.Where(r => r.DestinationPath == file.FullName);

        foreach (var actionKey in toRemove)
        {
            Hashes.Remove(actionKey);
        }

        base.Delete(file);
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
            DestinationFileSystemName = destinationFileSystemName,
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