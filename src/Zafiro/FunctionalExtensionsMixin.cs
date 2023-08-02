using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Zafiro;

public static class FunctionalExtensionsMixin
{
    public static Task<Result> Using<T>(this Result<T> result, Func<T, Task<Result>> d) where T : IDisposable
    {
        return result.Bind(async x =>
        {
            using (x)
            {
                return await d(x).ConfigureAwait(false);
            }
        });
    }

    public static Task<Result> Using<T, U>(this Result<T> result, Func<T, Task<Result>> d) where T : IDisposable
    {
        return result.Bind(async x =>
        {
            using (x)
            {
                return await d(x).ConfigureAwait(false);
            }
        });
    }
}