using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Zafiro.Mixins;

namespace Zafiro.CSharpFunctionalExtensions;

[PublicAPI]
public static class FunctionalMixin
{
    public static IObservable<Unit> Successes(this IObservable<Result> self)
    {
        return self.Where(a => a.IsSuccess).ToSignal();
    }

    public static IObservable<T> Successes<T>(this IObservable<Result<T>> self)
    {
        return self.Where(a => a.IsSuccess).Select(x => x.Value);
    }

    public static IObservable<bool> IsSuccess<T>(this IObservable<Result<T>> self)
    {
        return self.Select(a => a.IsSuccess);
    }

    public static IObservable<bool> IsSuccess(this IObservable<Result> self)
    {
        return self.Select(a => a.IsSuccess);
    }

    public static IObservable<bool> IsFailure(this IObservable<Result> self)
    {
        return self.Select(a => a.IsFailure);
    }

    public static IObservable<string> Failures(this IObservable<Result> self)
    {
        return self.Where(a => a.IsFailure).Select(x => x.Error);
    }

    public static IObservable<string> Failures<T>(this IObservable<Result<T>> self)
    {
        return self.Where(a => a.IsFailure).Select(x => x.Error);
    }

    public static IObservable<T> Values<T>(this IObservable<Maybe<T>> self)
    {
        return self.Where(x => x.HasValue).Select(x => x.Value);
    }

    public static IEnumerable<T> Values<T>(this IEnumerable<Maybe<T>> self)
    {
        return self.Where(x => x.HasValue).Select(x => x.Value);
    }

    public static IEnumerable<string> Failures(this IEnumerable<Result> self)
    {
        return self.Where(a => a.IsFailure).Select(x => x.Error);
    }

    public static IEnumerable<string> Failures<T>(this IEnumerable<Result<T>> self)
    {
        return self.Where(a => a.IsFailure).Select(x => x.Error);
    }

    public static IEnumerable<T> Successes<T>(this IEnumerable<Result<T>> self)
    {
        return self.Where(a => a.IsSuccess)
            .Select(x => x.Value);
    }

    public static IEnumerable<string> NotNullOrEmpty(this IEnumerable<string> self)
    {
        return self.Where(s => !string.IsNullOrWhiteSpace(s));
    }

    [Obsolete("You should not need this")]
    public static IObservable<Unit> Empties<T>(this IObservable<Maybe<T>> self)
    {
        return self.Where(x => !x.HasValue).Select(_ => Unit.Default);
    }

    public static bool AnyEmpty<T>(this IEnumerable<Maybe<T>> self)
    {
        return self.Any(x => x.HasNoValue);
    }

    public static Maybe<TResult> Combine<T, TResult>(this IList<Maybe<T>> values, Func<IEnumerable<T>, TResult> combinerFunc)
    {
        if (values.AnyEmpty())
        {
            return Maybe<TResult>.None;
        }

        return Maybe.From(combinerFunc(values.Select(maybe => maybe.Value)));
    }

    public static Maybe<T> AsMaybe<T>(this Result<T> result)
    {
        if (result.IsFailure)
        {
            return Maybe<T>.None;
        }

        return Maybe.From(result.Value);
    }

    public static async Task<Maybe<T>> AsMaybe<T>(this Task<Result<T>> resultTask)
    {
        return (await resultTask).AsMaybe();
    }

    public static Result<TCast> Cast<T, TCast>(this Result<T> result) where T : TCast
    {
        return result.Map(arg => (TCast) arg);
    }

    public static Task<Result<TCast>> Cast<T, TCast>(this Task<Result<T>> result) where T : TCast
    {
        return result.Map(arg => (TCast) arg);
    }

    public static Task<Result<Maybe<TResult>>> Bind<TFirst, TResult>(
        this Task<Result<Maybe<TFirst>>> task, 
        Func<TFirst, Task<Result<Maybe<TResult>>>> selector)
    {
        return task.Bind(maybe => maybe.Match(f => selector(f), () => Task.FromResult(Result.Success(Maybe<TResult>.None))));
    }

    // TODO: Test this
    public static Result<Maybe<TResult>> Bind<TFirst, TResult>(
        this Result<Maybe<TFirst>> task, 
        Func<TFirst, Result<Maybe<TResult>>> selector)
    {
        return task.Bind(maybe => maybe.Match(f => selector(f), () => Result.Success(Maybe<TResult>.None)));
    }
}