using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IZafiroFile : IStorable
{
    Task<Result> CopyTo(IZafiroFile destination);
    Result Delete();
    IZafiroFileSystem FileSystem { get; }
}