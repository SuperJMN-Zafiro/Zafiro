namespace Zafiro.FileSystem.Core;

public class DirectoryProperties
{
    public DirectoryProperties(bool isHidden, DateTimeOffset creationTime)
    {
        IsHidden = isHidden;
        CreationTime = creationTime;
    }

    public DateTimeOffset CreationTime { get; }
    public bool IsHidden { get; }
}