using System.IO.Abstractions;

namespace FileSystem;

public class FileComparer : IFileComparer
{
    public bool AreEqual(IFileInfo info, IFileInfo fileInfo)
    {
        throw new NotImplementedException();
    }
}