using System.IO.Abstractions;

namespace FileSystem;

public class Cache
{
    public Cache(IFileSystem fileSystem)
    {
            
    }

    public void Add(string path, Func<Stream> contents)
    {

    }

    public bool Contains(string path)
    {
        throw new NotImplementedException();
    }
}