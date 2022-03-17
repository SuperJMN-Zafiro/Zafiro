using CSharpFunctionalExtensions;
using Serilog;

namespace FileSystem;

public interface ICopier
{
    Task<Result> Copy(IZafiroDirectory source, IZafiroDirectory destination, Maybe<ILogger> logger);
}