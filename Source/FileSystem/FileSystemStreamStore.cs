using System.IO.Abstractions;

namespace FileSystem;

public class FileSystemStreamStore : IStreamStore
{
    private readonly IDirectoryInfo root;

    public FileSystemStreamStore(IDirectoryInfo root)
    {
        this.root = root;
    }

    public async Task Create(string path, Func<Stream> getContents)
    {
        await using var dest = root.FileSystem.FileInfo.FromFileName(path).OpenWrite();
        await getContents().CopyToAsync(dest);
    }

    public async Task Replace(string path, Func<Stream> getContents)
    {
        await using var dest = root.FileSystem.FileInfo.FromFileName(path).OpenWrite();
        await using var con = getContents();
        await con.CopyToAsync(dest);
    }

    public Task Delete(string path)
    {
        var file = root.FileSystem.FileInfo.FromFileName(path);
        file.Delete();
        return Task.CompletedTask;
    }
}