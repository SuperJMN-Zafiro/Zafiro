using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Zafiro.Misc;

public static class StringUtil
{
    public static async Task<Result<string>> GetNewName(string seed, Func<string, Task<Result<bool>>> isValid, Func<string, int, string> generateNewName)
    {
        return await TryName(seed, seed, 0);

        async Task<Result<string>> TryName(string name, string current, int i)
        {
            var result = await isValid(current);
        
            return result is { IsSuccess: true, Value: true } 
                ? Result.Success(current) 
                : result.IsFailure 
                    ? result.ConvertFailure<string>() 
                    : await TryName(seed, generateNewName(seed, i + 1), i+1);
        }
    }
}