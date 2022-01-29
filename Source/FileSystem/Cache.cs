using System.IO.Abstractions;
using System.Text.Json;

namespace FileSystem;

public class Cache
{
    private readonly IFileSystem fileSystem;
    private readonly string path;
    private readonly IHashGenerator hashGenerator;
    private Dictionary<string, byte[]> hashes = new();

    public Cache(IFileSystem fileSystem, string path, IHashGenerator hashGenerator)
    {
        this.fileSystem = fileSystem;
        this.path = path;
        this.hashGenerator = hashGenerator;
        Load(path);
    }

    private void Load(string directory)
    {
        var dir = fileSystem.DirectoryInfo.FromDirectoryName(directory);
        var file = fileSystem.FileInfo.FromFileName(fileSystem.Path.Combine(dir.FullName, "contents.hash"));
        if (file.Exists)
        {
            hashes = JsonSerializer.Deserialize<Dictionary<string, byte[]>>(fileSystem.File.ReadAllText(file.FullName));
        }
    }

    public async Task Add(string path, Func<Stream> contents)
    {
        hashes[path] = await hashGenerator.ComputeHash(contents);
    }

    private async Task<byte[]> ComputeHash(Func<Stream> streamFactory)
    {
        await using var stream = streamFactory();
        return await hashGenerator.ComputeHash(stream);
    }

    public async Task<bool> Contains(string path, Func<Stream> streamFactory)
    {
        var key = path;
        if (hashes.TryGetValue(key, out var existingHash))
        {
            return existingHash.SequenceEqual(await hashGenerator.ComputeHash(streamFactory));
        }

        return false;
    }

    public async Task Save()
    {
        //foreach (var dirCache in hashes)
        //{
        //    var dir = fileSystem.DirectoryInfo.FromDirectoryName(dirCache.Key);
        //    dir.Create();
        //    var file = fileSystem.FileInfo.FromFileName(fileSystem.Path.Combine(dir.FullName, "content.hash"));
        //    await using var stream = file.Create();
        //    await JsonSerializer.SerializeAsync(stream, dirCache);
        //}
    }
}