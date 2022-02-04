using System.IO.Abstractions;

namespace FileSystem;

public interface IFileManager
{
    Task Copy(IFileInfo source, IFileInfo destination);
    void Delete(IFileInfo file);
}