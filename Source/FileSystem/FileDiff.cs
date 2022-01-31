using System.IO.Abstractions;

namespace FileSystem;

public class FileDiff
{
    public FileDiff(IFileInfo source, FileDiffStatus status)
    {
        Source = source;
        Status = status;
    }

    public IFileInfo Source { get; }
    public FileDiffStatus Status { get; }

    public override string ToString()
    {
        return $"{nameof(Source)}: {Source}, {nameof(Status)}: {Status}";
    }
}