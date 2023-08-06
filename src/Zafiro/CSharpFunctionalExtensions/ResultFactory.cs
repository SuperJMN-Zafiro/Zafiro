using System;
using CSharpFunctionalExtensions;

namespace Zafiro.CSharpFunctionalExtensions;

public static class ResultFactory
{
    public static Result<K> Combine<T, Q, K>(this Result<T> one, Result<Q> another, Func<T, Q, K> combineFunction)
    {
        return one.Bind(x => another.Map(y => combineFunction(x, y)));
    }
}