namespace FileSystem;

public class FileDiff
{
    public FileDiff(string source, FileDiffStatus status)
    {
        Source = source;
        Status = status;
    }

    public string Source { get; }
    public FileDiffStatus Status { get; }

    public override string ToString()
    {
        return $"{nameof(Source)}: {Source}, {nameof(Status)}: {Status}";
    }
}