using CSharpFunctionalExtensions;
using Serilog;

namespace Zafiro.FileSystem;

public interface IZafiroFileSystem
{
    Result<IZafiroFile> GetFile(ZafiroPath path);
    Result<IZafiroDirectory> GetDirectory(ZafiroPath path);
    Maybe<ILogger> Logger { get; }
}