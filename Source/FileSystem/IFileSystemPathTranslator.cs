using System.IO.Abstractions;

namespace FileSystem;

public interface IFileSystemPathTranslator
{
    public string Translate(IFileSystemInfo fileSystemInfo, IFileSystemInfo origin, IDirectoryInfo destination);
}