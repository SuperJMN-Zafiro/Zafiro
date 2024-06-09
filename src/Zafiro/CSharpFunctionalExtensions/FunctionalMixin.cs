using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Serilog;
using Serilog.Core;
using Zafiro.Reactive;

namespace Zafiro.CSharpFunctionalExtensions;

[PublicAPI]
public static class FunctionalMixin
{
    public static IObservable<Unit> Successes(this IObservable<Result> self) => self.Where(a => a.IsSuccess).ToSignal();

    public static IObservable<T> Successes<T>(this IObservable<Result<T>> self) => self.Where(a => a.IsSuccess).Select(x => x.Value);

    public static IObservable<bool> IsSuccess<T>(this IObservable<Result<T>> self) => self.Select(a => a.IsSuccess);

    public static IObservable<bool> IsSuccess(this IObservable<Result> self) => self.Select(a => a.IsSuccess);

    public static IObservable<bool> IsFailure(this IObservable<Result> self) => self.Select(a => a.IsFailure);

    public static IObservable<string> Failures(this IObservable<Result> self) => self.Where(a => a.IsFailure).Select(x => x.Error);

    public static IObservable<string> Failures<T>(this IObservable<Result<T>> self) => self.Where(a => a.IsFailure).Select(x => x.Error);

    public static IObservable<T> Values<T>(this IObservable<Maybe<T>> self) => self.Where(x => x.HasValue).Select(x => x.Value);

    public static IEnumerable<T> Values<T>(this IEnumerable<Maybe<T>> self) => self.Where(x => x.HasValue).Select(x => x.Value);

    public static IEnumerable<string> Failures(this IEnumerable<Result> self) => self.Where(a => a.IsFailure).Select(x => x.Error);

    public static IEnumerable<string> Failures<T>(this IEnumerable<Result<T>> self) => self.Where(a => a.IsFailure).Select(x => x.Error);

    public static IEnumerable<T> Successes<T>(this IEnumerable<Result<T>> self) => self.Where(a => a.IsSuccess)
        .Select(x => x.Value);

    public static IEnumerable<string> NotNullOrEmpty(this IEnumerable<string> self) => self.Where(s => !string.IsNullOrWhiteSpace(s));

    /// <summary>
    /// Signals when the emitted item doesn't have a value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <returns></returns>
    public static IObservable<Unit> Empties<T>(this IObservable<Maybe<T>> self) => self.Where(x => !x.HasValue).Select(_ => Unit.Default);

    public static bool AnyEmpty<T>(this IEnumerable<Maybe<T>> self) => self.Any(x => x.HasNoValue);

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

    public static async Task<Maybe<T>> AsMaybe<T>(this Task<Result<T>> resultTask) => (await resultTask.ConfigureAwait(false)).AsMaybe();

    public static Result<TDestination> Cast<TSource, TDestination>(this Result<TSource> source, Func<TSource, TDestination> conversionFactory) => source.Map(conversionFactory);

    public static Task<Result<TDestination>> Cast<TSource, TDestination>(this Task<Result<TSource>> source, Func<TSource, TDestination> conversionFactory) => source.Map(conversionFactory);

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

    public static async Task<Result<IEnumerable<TResult>>> MapAndCombine<TInput, TResult>(
        this Result<IEnumerable<Task<Result<TInput>>>> enumerableOfTaskResults,
        Func<TInput, TResult> selector)
    {
        var result = await enumerableOfTaskResults.Map(async taskResults =>
        {
            var results = await Task.WhenAll(taskResults).ConfigureAwait(false);
            return results.Select(x => x.Map(selector)).Combine();
        }).ConfigureAwait(false);

        return result;
    }
    
    /// <summary>
    /// Binds and combines the results of the selector function applied to each item in the task of results.
    /// </summary>
    /// <typeparam name="T">The type of items in the input collection.</typeparam>
    /// <typeparam name="K">The type of items in the result collection.</typeparam>
    /// <param name="taskOfResults">A task that produces a Result of an IEnumerable of T.</param>
    /// <param name="selector">A function to apply to each item in the input collection.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Result of an IEnumerable of K.</returns>
    public static Task<Result<IEnumerable<K>>> BindAndCombine<T, K>(
        this Task<Result<IEnumerable<T>>> taskOfResults,
        Func<T, Task<Result<K>>> selector)
    {
        return taskOfResults.Bind(async inputs =>
        {
            var tasksOfResult = inputs.Select(selector);
            var results = await Task.WhenAll(tasksOfResult).ConfigureAwait(false);
            return results.Combine();
        });
    }
    
    public static async Task<Result> Using(this Task<Result<Stream>> streamResult, Func<Stream, Task> useStream)
    {
        return await streamResult.Tap(async stream =>
        {
            await using (stream.ConfigureAwait(false))
            {
                await useStream(stream).ConfigureAwait(false);
            }
        }).ConfigureAwait(false);
    }

    public static async Task<Maybe<Task>> Tap<T>(this Task<Maybe<T>> maybeTask, Action<T> action)
    {
        var maybe = await maybeTask.ConfigureAwait(false);
        
        if (maybe.HasValue)
        {
            action(maybe.Value);
        }
        
        return maybeTask;
    }
    
    public static Maybe<T> Tap<T>(this Maybe<T> maybe, Action<T> action)
    {
        if (maybe.HasValue)
        {
            action(maybe.Value);
        }
        
        return maybe;
    }
    
    /// <summary>
    /// Binds a collection of results to a function, and combines the results into a single task.
    /// </summary>
    /// <typeparam name="TInput">The type of the input values.</typeparam>
    /// <typeparam name="TResult">The type of the result values.</typeparam>
    /// <param name="taskResult">The task containing a collection of results.</param>
    /// <param name="selector">A function to apply to each result.</param>
    /// <returns>A task containing a collection of results after applying the selector function.</returns>
    public static Task<Result<IEnumerable<TResult>>> BindMany<TInput, TResult>(this Task<Result<IEnumerable<TInput>>> taskResult, Func<TInput, Result<TResult>> selector)
    {
        return taskResult.Bind(inputs => inputs.Select(selector).Combine());
    }

    public static async Task<Result<IEnumerable<TResult>>> Combine<TResult>(this IEnumerable<Task<Result<TResult>>> enumerableOfTaskResults)
    {
        var whenAll = await Task.WhenAll(enumerableOfTaskResults);
        return whenAll.Combine();
    }
    
    public static Task<Result<IEnumerable<TResult>>> Combine<TResult>(this Task<Result<IEnumerable<Task<Result<TResult>>>>> task)
    {
        return task.Bind(async tasks => await Combine(tasks));
    }
    
    /// <summary>
    /// Transforms the results of a task using a provided selector function.
    /// </summary>
    /// <typeparam name="TInput">The type of the input values.</typeparam>
    /// <typeparam name="TResult">The type of the result values.</typeparam>
    /// <param name="taskResult">The task containing a collection of results.</param>
    /// <param name="selector">A function to apply to each result.</param>
    /// <returns>A task containing a collection of results after applying the selector function.</returns>
    public static Task<Result<IEnumerable<TResult>>> Map<TInput, TResult>(this Task<Result<IEnumerable<TInput>>> taskResult, Func<TInput, TResult> selector)
    {
        return taskResult.Map(inputs => inputs.Select(selector));
    }

    public static Task<Result<IEnumerable<TResult>>> BindMany<TInput, TResult>(this Task<Result<IEnumerable<TInput>>> taskResult, Func<TInput, Task<Result<TResult>>> selector)
    {
        return taskResult.Bind(inputs => AsyncResultExtensionsLeftOperand.Combine(inputs.Select(selector)));
    }

    public static void Log(this Result result, ILogger logger, string successString = "Success")
    {
        result
            .Tap(() => logger.Information(successString))
            .TapError(Serilog.Log.Error);
    }
    
    public static async Task Log(this Task<Result> result, ILogger? logger = default, string successString = "Success")
    {
        (await result.ConfigureAwait(false)).Log(logger ?? Serilog.Log.Logger, successString);
    }
}