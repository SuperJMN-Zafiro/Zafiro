using System.Collections;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Core;

public class HashCompareStrategy : IFileCompareStrategy
{
    public async Task<Result<bool>> Compare(IZafiroFile one, IZafiroFile another)
    {
        var areEqual = await from leftHashes in one.Hashes
            from rightHashes in another.Hashes
            select from leftHash in leftHashes
                join rightHash in rightHashes on leftHash.Key equals rightHash.Key
                select new { LeftHash = leftHash, RightHash = rightHash };

        return areEqual.Map(x => x.Any(combination => StructuralComparisons.StructuralEqualityComparer.Equals(combination.LeftHash.Value, combination.RightHash.Value)));
    }
}