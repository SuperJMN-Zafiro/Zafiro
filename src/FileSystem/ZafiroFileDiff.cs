using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public class ZafiroFileDiff
{
    public ZafiroFileDiff(Maybe<IZafiroFile> left, Maybe<IZafiroFile> right)
    {
        Left = left;
        Right = right;
    }

    public Maybe<IZafiroFile> Left { get; }
    public Maybe<IZafiroFile> Right { get; }


    public FileDiffStatus Status => Left.HasValue && Right.HasValue ? FileDiffStatus.Both :
        Left.HasValue ? FileDiffStatus.LeftOnly : FileDiffStatus.RightOnly;

    public override string ToString()
    {
        return $"{nameof(Left)}: {Left}, {nameof(Right)}: {Right}";
    }
}