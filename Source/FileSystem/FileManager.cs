using System.IO.Abstractions;

namespace FileSystem;

public class FileManager : IFileManager
{
    public virtual async Task Copy(IFileInfo source, IFileInfo destination)
    {
        await using var openWrite = destination.OpenWrite();
        await using var openRead = source.OpenRead();
        await openRead.CopyToAsync(openWrite);
    }

    public virtual void Delete(IFileInfo file)
    {
        file.Delete();
    }
}