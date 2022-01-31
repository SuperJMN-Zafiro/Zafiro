using System.IO.Abstractions;

namespace FileSystem;

public class BulkCopier
{
    private readonly FileSystemComparer fileSystemComparer;
    private readonly IFileSystemPathTranslator pathTranslator;

    public BulkCopier(FileSystemComparer systemComparer, IFileSystemPathTranslator pathTranslator)
    {
        fileSystemComparer = systemComparer;
        this.pathTranslator = pathTranslator;
    }

    public async Task Copy(IDirectoryInfo a, IDirectoryInfo b, IStreamStore store)
    {
        var diffs = await fileSystemComparer.Diff(a, b);
        foreach (var diff in diffs)
        {
            var translatedPath = pathTranslator.Translate(diff.Source, a, b);
            var destFile = b.FileSystem.FileInfo.FromFileName(translatedPath);

            switch (diff.Status)
            {
                case FileDiffStatus.Created:
                    await store.Delete(destFile.FullName);
                    break;
                case FileDiffStatus.Modified:
                    await store.Replace(destFile.FullName, diff.Source.OpenRead);
                    break;
                case FileDiffStatus.Deleted:
                    destFile.Directory.Create();
                    await store.Create(destFile.FullName, diff.Source.OpenRead);
                    break;
            }
        }
    }
}

public interface IStreamStore
{
    public Task Create(string path, Func<Stream> getContents);
    public Task Replace(string path, Func<Stream> getContents);
    public Task Delete(string path);
}

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
        using (var con = getContents())
        {
            var a = await new StreamReader(getContents()).ReadToEndAsync();
            await con.CopyToAsync(dest);
        }
    }

    public Task Delete(string path)
    {
        var file = root.FileSystem.FileInfo.FromFileName(path);
        file.Delete();
        return Task.CompletedTask;
    }
}