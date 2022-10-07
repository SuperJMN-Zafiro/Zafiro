using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IZafiroFile
{
    ZafiroPath Path { get; }
    Task<Result> CopyTo(IZafiroFile destination);
    Task<Stream> OpenWrite();
    Result Delete();
    Task<Stream> OpenRead();
    IZafiroFileSystem FileSystem { get; }
}