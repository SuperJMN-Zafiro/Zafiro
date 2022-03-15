using CSharpFunctionalExtensions;

namespace FileSystem;

public interface IZafiroFileSystem
{
    Result<IZafiroFile> GetFile(ZafiroPath path);
    Result<IZafiroDirectory> GetDirectory(ZafiroPath path);
}