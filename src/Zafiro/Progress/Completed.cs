namespace Zafiro.Progress;

public class Completed : IProgress
{
    public static Completed Instance { get; } = new();
}