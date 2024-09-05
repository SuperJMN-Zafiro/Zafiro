using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Comparer;

public record RightOnlyDiff : FileDiff
{
    public RightOnlyDiff(IRootedFile right)
    {
        Right = right;
    }

    public IRootedFile Right { get; }
}