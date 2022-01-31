using System.IO.Abstractions;

namespace FileSystem;

public class DiffContext
{
    public DiffContext(IFileSystemInfo origin, IFileSystemInfo destination)
    {
        Origin = origin;
        Destination = destination;
    }

    public IFileSystemInfo Origin { get; set; }
    public IFileSystemInfo Destination { get; set; }
}