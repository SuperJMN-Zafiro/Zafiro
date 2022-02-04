using System.IO.Abstractions;

namespace FileSystem;

public interface IFileSystemComparer
{
    Task<IEnumerable<FileDiff>> Diff(IDirectoryInfo origin, IDirectoryInfo destination);
}