using CSharpFunctionalExtensions;

namespace FileSystem;

public interface IZafiroFile
{
    ZafiroPath Path { get; }
    Task<Result> CopyTo(IZafiroFile destination);
    Stream OpenWrite();
    Result Delete();
    Stream OpenRead();
    IZafiroFileSystem FileSystem { get; }
}