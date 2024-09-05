using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Core;

public class SizeCompareStrategy : IFileCompareStrategy
{
    public Task<Result<bool>> Compare(IZafiroFile one, IZafiroFile another)
    {
        var o = from l in one.Properties from r in another.Properties select new { l, r };
        return o.Map(x => x.l.Length == x.r.Length);
    }
}