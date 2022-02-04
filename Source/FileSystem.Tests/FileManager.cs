using System.IO.Abstractions;
using System.Threading.Tasks;

namespace FileSystem.Tests;

internal class FileManager : IFileManager
{
    public async Task Copy(IFileInfo source, IFileInfo destination)
    {
        await using var openWrite = destination.OpenWrite();
        await using var openRead = source.OpenRead();
        await openRead.CopyToAsync(openWrite);
    }

    public void Delete(IFileInfo file)
    {
        file.Delete();
    }
}