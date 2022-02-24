using CSharpFunctionalExtensions;

namespace FileSystem;

public interface ICopier
{
    Task<Result> Copy(IZafiroDirectory source, IZafiroDirectory destination);
}