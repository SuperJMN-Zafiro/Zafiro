using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Comparer;

public record LeftOnlyDiff : FileDiff
{
    public LeftOnlyDiff(IRootedFile left)
    {
        Left = left;
    }

    public IRootedFile Left { get; }
}