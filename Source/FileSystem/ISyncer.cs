using CSharpFunctionalExtensions;

namespace FileSystem;

public interface ISyncer
{
    Task<Result> Sync(IZafiroDirectory source, IZafiroDirectory destination);
}