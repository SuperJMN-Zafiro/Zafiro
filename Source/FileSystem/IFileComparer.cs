using System.IO.Abstractions;

namespace FileSystem;

public interface IFileComparer
{
    bool AreEqual(IFileInfo info, IFileInfo fileInfo);
}