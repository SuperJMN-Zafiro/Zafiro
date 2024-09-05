using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Core;

public interface IFileCompareStrategy
{
    Task<Result<bool>> Compare(IZafiroFile one, IZafiroFile another);
}