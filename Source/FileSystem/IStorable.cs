namespace Zafiro.FileSystem;

public interface IStorable
{
    ZafiroPath Path { get; }
    string Name { get; }
    Task<Stream> OpenWrite();
    Task<Stream> OpenRead();
}