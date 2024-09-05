using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Readonly;

namespace Zafiro.FileSystem.Core;

public static class AsyncDirMixin
{
    public static Task<Result<IEnumerable<IFile>>> Files(this IAsyncDir dir)
    {
        return dir.Children().Map(x => x.OfType<IFile>());
    }

    public static Task<Result<IEnumerable<IDirectory>>> Directories(this IAsyncDir dir)
    {
        return dir.Children().Map(x => x.OfType<IDirectory>());
    }
}