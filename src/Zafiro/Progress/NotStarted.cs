namespace Zafiro.Progress;

public class NotStarted : IProgress
{
    public static NotStarted Instance { get; } = new();
}