using System;
using CSharpFunctionalExtensions;

namespace Zafiro.CSharpFunctionalExtensions;

public static class MaybeMixin
{
    public static Result<TResult> MapMaybe<T, TResult>(this Maybe<Result<T>> maybeResult, Func<Maybe<T>, TResult> selector)
    {
        return maybeResult.Match(result =>
        {
            return result.Match(icon =>
            {
                var value = selector(icon);
                return Result.Success(value);
            }, Result.Failure<TResult>);
        }, () =>
        {
            var result = selector(Maybe.None);
            return Result.Success(result);
        });
    }
}