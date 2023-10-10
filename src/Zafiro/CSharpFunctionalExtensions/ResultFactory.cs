using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Zafiro.CSharpFunctionalExtensions;

public static class ResultFactory
{
    public static Result<K> CombineAndMap<T, Q, K>(this Result<T> one, Result<Q> another, Func<T, Q, K> combineFunction)
    {
        return one.Bind(x => another.Map(y => combineFunction(x, y)));
    }

    public static Task<Result<K>> CombineAndMap<T, Q, K>(this Task<Result<T>> one, Task<Result<Q>> another, Func<T, Q, K> combineFunction)
    {
        return one.Bind(x => another.Map(y => combineFunction(x, y)));
    }

    public static Task<Result<K>> CombineAndBind<T, Q, K>(this Task<Result<T>> one, Task<Result<Q>> another, Func<T, Q, Result<K>> combineFunction)
    {
        return one.Bind(x => another.Bind(y => combineFunction(x, y)));
    }

    public static Task<Result> CombineAndBind<T, Q>(this Task<Result<T>> one, Task<Result<Q>> another, Func<T, Q, Result> combineFunction)
    {
        return one.Bind(x => another.Bind(y => combineFunction(x, y)));
    }

    public static Task<Result> CombineAndBind<T, Q>(this Task<Result<T>> one, Task<Result<Q>> another, Func<T, Q, Task<Result>> combineFunction)
    {
        return one.Bind(x => another.Bind(y => combineFunction(x, y)));
    }

    public static Task<Result<K>> CombineAndBind<T, Q, K>(this Task<Result<T>> one, Task<Result<Q>> another, Func<T, Q, Task<Result<K>>> combineFunction)
    {
        return one.Bind(x => another.Bind(y => combineFunction(x, y)));
    }
}