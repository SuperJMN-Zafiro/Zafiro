namespace Zafiro.FileSystem;

public interface IStorable
{
    ZafiroPath Path { get; }
    Task<Stream> OpenWrite();
    Task<Stream> OpenRead();
}