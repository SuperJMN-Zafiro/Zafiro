using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;

namespace Zafiro.CSharpFunctionalExtensions;

public static class ReactiveResultMixin
{
    public static IObservable<Result<K>> Map<T, K>(this IObservable<Result<T>> observable, Func<T, K> function)
    {
        return observable.Select(t => t.Map(function));
    }

    public static IObservable<Result<K>> Map<T, K>(this IObservable<Result<T>> observable, Func<T, Task<K>> function)
    {
        return observable.SelectMany(t => t.Map(function));
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
}