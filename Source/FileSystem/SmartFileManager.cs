using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Text.Json;
using CSharpFunctionalExtensions;

namespace FileSystem;

public class SmartFileManager : FileManager
{
    private static readonly SHA1 Hasher = SHA1.Create();
    private readonly IFileInfo databaseFile;
    public readonly Dictionary<string, byte[]> hashes = new();

    public SmartFileManager(IFileInfo databaseFile)
    {
        this.databaseFile = databaseFile;
    }

    public override async Task Copy(IFileInfo source, IFileInfo destination)
    {
        var hashedFile = new HashedFile(await GetHash(source.OpenRead), source);
        if (AreEqual(hashedFile, destination)) return;

        await base.Copy(source, destination);
        hashes[ToKey(source, destination)] = hashedFile.Hash;
    }

    public async Task Save()
    {
        await using var openRead = databaseFile.Create();
        await JsonSerializer.SerializeAsync(openRead, hashes);
    }

    public override void Delete(IFileInfo file)
    {
        var toRemove = hashes.Keys.Where(r => FromKey(r).destination == file.FullName);

        foreach (var actionKey in toRemove) hashes.Remove(actionKey);

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

        return hashes
            .TryFind(key)
            .Match(destHash => source.Hash.SequenceEqual(destHash), () => false);
    }

    private string ToKey(IFileInfo sourceFile, IFileInfo destination)
    {
        return sourceFile.FullName + ";" + destination.FullName;
    }

    private (string sourceFile, string destination) FromKey(string key)
    {
        var indexOf = key.IndexOf(";", StringComparison.InvariantCulture);
        return (key[indexOf..], key[..indexOf]);
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