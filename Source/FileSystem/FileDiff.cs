using System.IO.Abstractions;

namespace FileSystem;

public class FileDiff
{
    public FileDiff(IFileInfo left, IFileInfo right)
    {
        Left = left;
        Right = right;
    }

    public IFileInfo? Left { get; }
    public IFileInfo? Right { get; }

    public FileDiffStatus Status =>
        Left is null && Right is null ? FileDiffStatus.Invalid
        : Left is null ? FileDiffStatus.RightOnly
        : Right is null ? FileDiffStatus.LeftOnly : FileDiffStatus.Both;

    public override string ToString()
    {
        return $"{nameof(Left)}: {Left}, {nameof(Right)}: {Right}";
    }
}