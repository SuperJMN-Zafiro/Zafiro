namespace Zafiro.FileSystem.Core;

public record Rooted<T> : IRooted<T>
{
    public Rooted(ZafiroPath path, T value)
    {
        Path = path;
        Value = value;
    }

    public ZafiroPath Path { get; }
    public T Value { get; }
}

public record Rooted
{
    public static Rooted<T> Create<T>(ZafiroPath path, T value)
    {
        return new Rooted<T>(path, value);
    }
}