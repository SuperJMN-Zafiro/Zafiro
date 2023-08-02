using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface ISyncer
{
    Task<Result> Sync(IZafiroDirectory source, IZafiroDirectory destination);
}