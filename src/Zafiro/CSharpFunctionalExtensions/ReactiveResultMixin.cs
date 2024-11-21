using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Zafiro.CSharpFunctionalExtensions;

public static class ReactiveResultMixin
{
    public static IObservable<Result<K>> Map<T, K>(this IObservable<Result<T>> observable, Func<T, K> function)
    {
        return observable.Select(t => t.Map(function));
    }

    public static IObservable<Result<K>> Map<T, K>(this IObservable<Result<T>> observable, Func<T, Task<K>> function)
    {
        return observable.SelectMany(t => AsyncResultExtensionsRightOperand.Map(t, function));
    }

    public static IObservable<Result<K>> Bind<T, K>(this IObservable<Result<T>> observable, Func<T, Task<Result<K>>> function)
    {
        return observable.SelectMany(t => t.Bind(function));
    }

    public static IObservable<Result<K>> Bind<T, K>(this IObservable<Result<T>> observable, Func<T, Result<K>> function)
    {
        return observable.Select(t => t.Bind(function));
    }

    public static IObservable<Result<K>> SelectMany<T, K>(this IObservable<Result<T>> observable, Func<T, IObservable<Result<K>>> function)
    {
        return observable
            .SelectMany(result => result.IsSuccess
                ? function(result.Value)
                : Observable.Return(Result.Failure<K>(result.Error)))
            .Catch((Exception ex) => Observable.Return(Result.Failure<K>(ex.Message)));
    }

    public static IObservable<Result> SelectMany<T>(this IObservable<Result<T>> observable, Func<T, IObservable<Result>> function)
    {
        return observable
            .SelectMany(result => result.IsSuccess
                ? function(result.Value)
                : Observable.Return(Result.Failure(result.Error)))
            .Catch((Exception ex) => Observable.Return(Result.Failure(ex.Message)));
    }

    public static IObservable<Result<R>> SelectMany<T, K, R>(this IObservable<Result<T>> observable, Func<T, IObservable<Result<K>>> collectionSelector, Func<T, K, R> resultSelector)
    {
        return observable.SelectMany(result => result.IsSuccess
            ? collectionSelector(result.Value)
            : Observable.Empty<Result<K>>(), (result, result1) => result.CombineAndMap(result1, resultSelector));
    }

    public static Task<Result> TapIf(this Task<Result> resultTask, Task<Result<bool>> conditionResult, Func<Task> func)
    {
        return conditionResult.Bind(condition => resultTask.TapIf(condition, func));
    }

    public static Task<Result> TapIf(this Task<Result> resultTask, Task<Result<bool>> conditionResult, Action action)
    {
        return conditionResult.Bind(condition => resultTask.TapIf(condition, action));
    }

    public static Task<Result<bool>> Not(this Task<Result<bool>> result)
    {
        return result.Map(b => !b);
    }

    public static async Task<Result> TapIf(this Result result, Task<Result<bool>> condition, Action action)
    {
        return ResultExtensions.Map(result, async () => { await condition.Tap(action).ConfigureAwait(false); });
    }

    public static async Task<Result> TapIfB(this Result result, Task<bool> condition, Func<Task> func)
    {
        return await condition.ConfigureAwait(false) ? await result.Tap(func).ConfigureAwait(false) : await Task.FromResult(result).ConfigureAwait(false);
    }

    public static async Task<Result<T>> TapIf<T>(this Result<T> result, Task<bool> condition, Func<Task<T>> func)
    {
        return await condition.ConfigureAwait(false) ? await result.Tap(func).ConfigureAwait(false) : await Task.FromResult(result).ConfigureAwait(false);
    }

    public static async Task<Result> TapIf(this Task<Result> resultTask, Task<bool> condition, Func<Task> func)
    {
        return await condition.ConfigureAwait(false) ? await resultTask.Tap(func).ConfigureAwait(false) : await resultTask.ConfigureAwait(false);
    }

    public static async Task<Result> TapIf(this Task<Result> resultTask, Task<bool> condition, Action action)
    {
        return await condition.ConfigureAwait(false) ? await resultTask.Tap(action).ConfigureAwait(false) : await resultTask.ConfigureAwait(false);
    }

    public static async Task<Result> Map<T>(this Task<Result<T>> resultTask, Func<T, Task> func)
    {
        return await (await resultTask.ConfigureAwait(false)).Map(func).ConfigureAwait(false);
    }

    public static async Task<Result> Map<T>(this Result<T> result, Func<T, Task> func)
    {
        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        await func(result.Value).ConfigureAwait(false);
        return Result.Success();
    }

    public static async Task<Result> Map(this Result result, Func<Task> func)
    {
        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        await func().ConfigureAwait(false);
        return Result.Success();
    }

    public static Result<IEnumerable<TResult>> MapEach<TInput, TResult>(this Result<IEnumerable<TInput>> input, Func<TInput, TResult> selector)
    {
        return input.Map(x => x.Select(selector));
    }

    public static Maybe<IEnumerable<TResult>> MapEach<TInput, TResult>(this Maybe<IEnumerable<TInput>> input, Func<TInput, TResult> selector)
    {
        return input.Map(x => x.Select(selector));
    }

    public static Task<Result> UnrollBind(this Result<Task<Result>> result, Action? prepend = null)
    {
        return result.Bind(async task =>
        {
            prepend?.Invoke();
            return await task.ConfigureAwait(false);
        });
    }

    public static Task UnrollMap(this Result<Task<Result>> result)
    {
        return result.Map(task => task);
    }
}