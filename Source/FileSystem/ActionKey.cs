using System.IO.Abstractions;

namespace FileSystem;

public class ActionKey
{
    public ActionKey()
    {
    }

    public ActionKey(IFileSystemInfo sourceFile, IFileSystemInfo destination)
    {
        SourceFile = sourceFile.FullName;
        Destination = destination.FullName;
    }

    public string SourceFile { get; set; }

    public string Destination { get; set; }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ActionKey) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SourceFile, Destination);
    }

    protected bool Equals(ActionKey other)
    {
        return SourceFile == other.SourceFile && Destination == other.Destination;
    }
}