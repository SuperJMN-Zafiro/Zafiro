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
        if (await AreEqual(source, destination)) return;

        await base.Copy(source, destination);
    }

    public async Task Save()
    {
        await using var openRead = databaseFile.Create();
        await JsonSerializer.SerializeAsync(openRead, hashes);
    }

    private static async Task<byte[]> GetHash(Func<Stream> getContents)
    {
        await using var stream = getContents();
        return await Hasher.ComputeHashAsync(stream);
    }

    private async Task<bool> AreEqual(IFileInfo source, IFileInfo destination)
    {
        var sourceHash = await GetHash(source.OpenRead);
        return hashes
            .TryFind(destination.FullName)
            .Match(destHash => sourceHash.SequenceEqual(destHash), () => false);
    }

    private async Task<bool> Exists(string path, Func<Stream> getContents)
    {
        var newHash = await GetHash(getContents);
        return hashes
            .TryFind(path)
            .Match(bytes => newHash.SequenceEqual(bytes), () => false);
    }

    private async Task<Dictionary<string, byte[]>> Load(IFileSystem fileSystem, string dirPath)
    {
        if (databaseFile.Exists)
        {
            await using var openRead = databaseFile.OpenRead();
            return await JsonSerializer.DeserializeAsync<Dictionary<string, byte[]>>(openRead);
        }

        return new Dictionary<string, byte[]>();
    }
}