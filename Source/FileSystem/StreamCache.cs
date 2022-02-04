using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Text.Json;
using CSharpFunctionalExtensions;

namespace FileSystem;

public class StreamCache
{
    private const string IndexFilename = "contents.hash";
    private static readonly SHA1 Hasher = SHA1.Create();
    private readonly IFileSystem fileSystem;
    private readonly Dictionary<string, byte[]> hashes;

    private StreamCache(IFileSystem fileSystem, Dictionary<string, byte[]> hashes)
    {
        this.fileSystem = fileSystem;
        this.hashes = hashes;
    }

    public static async Task<StreamCache> CreateInstance(IFileSystem fileSystem, string dirPath)
    {
        return new StreamCache(fileSystem, await Load(fileSystem, dirPath));
    }

    public static async Task Save(StreamCache cache, IFileSystem fileSystem, string dirPath)
    {
        var file = GetStorage(fileSystem, dirPath);
        await using var openRead = file.Create();
        await JsonSerializer.SerializeAsync(openRead, cache.hashes);
    }

    public async Task Store(string path, Func<Stream> getContents)
    {
        if (await Exists(path, getContents)) return;

        await using var stream = fileSystem.FileStream.Create(path, FileMode.Create);
        await using var source = getContents();
        await source.CopyToAsync(stream);

        await SaveHash(path, getContents);
    }

    private static IFileInfo GetStorage(IFileSystem fileSystem, string dirPath)
    {
        var dir = fileSystem.DirectoryInfo.FromDirectoryName(dirPath);
        return fileSystem.FileInfo.FromFileName(fileSystem.Path.Combine(dir.FullName, IndexFilename));
    }

    private static async Task<byte[]> GetHash(Func<Stream> getContents)
    {
        await using var stream = getContents();
        return await Hasher.ComputeHashAsync(stream);
    }

    private static async Task<Dictionary<string, byte[]>> Load(IFileSystem fileSystem, string dirPath)
    {
        var file = GetStorage(fileSystem, dirPath);
        if (file.Exists)
        {
            await using var openRead = file.OpenRead();
            return await JsonSerializer.DeserializeAsync<Dictionary<string, byte[]>>(openRead);
        }

        return new Dictionary<string, byte[]>();
    }

    private async Task SaveHash(string path, Func<Stream> getContents)
    {
        hashes[path] = await GetHash(getContents);
    }

    private async Task<bool> Exists(string path, Func<Stream> getContents)
    {
        var newHash = await GetHash(getContents);
        return hashes
            .TryFind(path)
            .Match(bytes => newHash.SequenceEqual(bytes), () => false);
    }
}