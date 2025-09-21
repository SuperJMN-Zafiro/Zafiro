using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Zafiro.CSharpFunctionalExtensions
{
    public static class ReactiveResultMixin
    {
        /// <summary>
        /// Projects each successful Result value of type <typeparamref name="T"/> in the observable sequence into a new Result of type <typeparamref name="K"/>
        /// by applying the specified synchronous <paramref name="function"/>.
        /// </summary>
        /// <typeparam name="T">Type of the input Result value.</typeparam>
        /// <typeparam name="K">Type of the output Result value.</typeparam>
        /// <param name="observable">Source sequence of Result&lt;T&gt; values.</param>
        /// <param name="function">Synchronous transformation to apply on each successful value.</param>
        /// <returns>An observable sequence of Result&lt;K&gt; values.</returns>
        /// <remarks>
        /// Use this method when you need to transform the payload of successful Results in a reactive pipeline.
        /// Failures propagate without invoking the <paramref name="function"/>.
        /// </remarks>
        public static IObservable<Result<K>> Map<T, K>(this IObservable<Result<T>> observable, Func<T, K> function)
        {
            return observable.Select(t => t.Map(function));
        }

        /// <summary>
        /// Projects each successful Result value of type <typeparamref name="T"/> into a new Result of type <typeparamref name="K"/>
        /// by applying the specified asynchronous <paramref name="function"/>.
        /// </summary>
        /// <typeparam name="T">Type of the input Result value.</typeparam>
        /// <typeparam name="K">Type of the output Result value.</typeparam>
        /// <param name="observable">Source sequence of Result&lt;T&gt; values.</param>
        /// <param name="function">Asynchronous transformation to apply on each successful value.</param>
        /// <returns>An observable sequence of Result&lt;K&gt; values.</returns>
        /// <remarks>
        /// This overload is suitable when the mapping requires asynchronous operations, such as I/O or network calls.
        /// </remarks>
        public static IObservable<Result<K>> Map<T, K>(this IObservable<Result<T>> observable, Func<T, Task<K>> function)
        {
            return observable.SelectMany(t => AsyncResultExtensionsRightOperand.Map(t, function));
        }

        /// <summary>
        /// Flattens and binds each successful Result value of type <typeparamref name="T"/> into a new asynchronous Result&lt;K&gt; via the specified <paramref name="function"/>.
        /// </summary>
        /// <typeparam name="T">Type of the input Result value.</typeparam>
        /// <typeparam name="K">Type of the output Result value.</typeparam>
        /// <param name="observable">Source sequence of Result&lt;T&gt; values.</param>
        /// <param name="function">Asynchronous binder returning Result&lt;K&gt;.</param>
        /// <returns>An observable sequence of Result&lt;K&gt; values after binding.</returns>
        /// <remarks>
        /// Use this method to chain asynchronous operations that return Result&lt;K&gt;, preserving failure propagation.
        /// </remarks>
        public static IObservable<Result<K>> Bind<T, K>(this IObservable<Result<T>> observable, Func<T, Task<Result<K>>> function)
        {
            return observable.SelectMany(t => t.Bind(function));
        }

        /// <summary>
        /// Flattens and binds each successful Result value of type <typeparamref name="T"/> into a new synchronous Result&lt;K&gt; via the specified <paramref name="function"/>.
        /// </summary>
        /// <typeparam name="T">Type of the input Result value.</typeparam>
        /// <typeparam name="K">Type of the output Result value.</typeparam>
        /// <param name="observable">Source sequence of Result&lt;T&gt; values.</param>
        /// <param name="function">Synchronous binder returning Result&lt;K&gt;.</param>
        /// <returns>An observable sequence of Result&lt;K&gt; values after binding.</returns>
        /// <remarks>
        /// This overload suits scenarios where binding logic is purely in-memory without asynchronous calls.
        /// </remarks>
        public static IObservable<Result<K>> Bind<T, K>(this IObservable<Result<T>> observable, Func<T, Result<K>> function)
        {
            return observable.Select(t => t.Bind(function));
        }

        /// <summary>
        /// Projects each successful Result value of type <typeparamref name="T"/> into a new observable sequence of Result&lt;K&gt; via the specified <paramref name="function"/>,
        /// flattening the resulting sequences into one.
        /// </summary>
        /// <typeparam name="T">Type of the input Result value.</typeparam>
        /// <typeparam name="K">Type of the inner sequence Result value.</typeparam>
        /// <param name="observable">Source sequence of Result&lt;T&gt; values.</param>
        /// <param name="function">Transformation producing an observable of Result&lt;K&gt; for each value.</param>
        /// <returns>An observable sequence of Result&lt;K&gt; values.</returns>
        /// <remarks>
        /// On failure of the inner observable or exception, the sequence emits a failure Result&lt;K&gt; with the exception message.
        /// Use this for branching reactive workflows based on Result values.
        /// </remarks>
        public static IObservable<Result<K>> SelectMany<T, K>(this IObservable<Result<T>> observable, Func<T, IObservable<Result<K>>> function)
        {
            return observable
                .SelectMany(result => result.IsSuccess
                    ? function(result.Value)
                    : Observable.Return(Result.Failure<K>(result.Error)))
                .Catch((Exception ex) => Observable.Return(Result.Failure<K>(ex.Message)));
        }

        /// <summary>
        /// Projects each successful Result value of type <typeparamref name="T"/> into a new observable sequence of Result via the specified <paramref name="function"/>,
        /// flattening the resulting sequences into one.
        /// </summary>
        /// <typeparam name="T">Type of the input Result value.</typeparam>
        /// <param name="observable">Source sequence of Result&lt;T&gt; values.</param>
        /// <param name="function">Transformation producing an observable of Result for each value.</param>
        /// <returns>An observable sequence of Result values.</returns>
        /// <remarks>
        /// Suited for reactive workflows where the continuation does not produce a value.
        /// </remarks>
        public static IObservable<Result> SelectMany<T>(this IObservable<Result<T>> observable, Func<T, IObservable<Result>> function)
        {
            return observable
                .SelectMany(result => result.IsSuccess
                    ? function(result.Value)
                    : Observable.Return(Result.Failure(result.Error)))
                .Catch((Exception ex) => Observable.Return(Result.Failure(ex.Message)));
        }

        /// <summary>
        /// Projects each successful Result value of type <typeparamref name="T"/> into an observable of Result&lt;K&gt;,
        /// then combines the original and inner Result values into a Result&lt;R&gt; via <paramref name="resultSelector"/>.
        /// </summary>
        /// <typeparam name="T">Type of the input Result value.</typeparam>
        /// <typeparam name="K">Type of the inner sequence Result value.</typeparam>
        /// <typeparam name="R">Type of the combined Result value.</typeparam>
        /// <param name="observable">Source sequence of Result&lt;T&gt; values.</param>
        /// <param name="collectionSelector">Transformation to an observable of Result&lt;K&gt;.</param>
        /// <param name="resultSelector">Combination function of T and K to produce an R.</param>
        /// <returns>An observable sequence of Result&lt;R&gt; values.</returns>
        /// <remarks>
        /// Facilitates joining two asynchronous pipelines of Results into one cohesive stream.
        /// </remarks>
        public static IObservable<Result<R>> SelectMany<T, K, R>(this IObservable<Result<T>> observable, Func<T, IObservable<Result<K>>> collectionSelector, Func<T, K, R> resultSelector)
        {
            return observable.SelectMany(result => result.IsSuccess
                ? collectionSelector(result.Value)
                : Observable.Empty<Result<K>>(), (result, result1) => result.CombineAndMap(result1, resultSelector));
        }

        /// <summary>
        /// Performs a side-effect <paramref name="func"/> if both the outer Result and <paramref name="conditionResult"/> succeed.
        /// </summary>
        /// <param name="resultTask">Asynchronous Result task to continue if condition passes.</param>
        /// <param name="conditionResult">Asynchronous condition returning Result&lt;bool&gt;.</param>
        /// <param name="func">Asynchronous side-effect to invoke on success.</param>
        /// <returns>A Task of Result after condition and side-effect.
        /// </returns>
        /// <remarks>
        /// Enables conditional execution of async actions based on multiple Result outcomes.
        /// </remarks>
        public static Task<Result> TapIf(this Task<Result> resultTask, Task<Result<bool>> conditionResult, Func<Task> func)
        {
            return conditionResult.Bind(condition => resultTask.TapIf(condition, func));
        }

        /// <summary>
        /// Performs a side-effect <paramref name="action"/> if both the outer Result and <paramref name="conditionResult"/> succeed.
        /// </summary>
        /// <param name="resultTask">Asynchronous Result task to continue if condition passes.</param>
        /// <param name="conditionResult">Asynchronous condition returning Result&lt;bool&gt;.</param>
        /// <param name="action">Synchronous side-effect to invoke on success.</param>
        /// <returns>A Task of Result after condition and side-effect.
        /// </returns>
        /// <remarks>
        /// Suitable when side-effect is synchronous and you want to conditionally augment a Result pipeline.
        /// </remarks>
        public static Task<Result> TapIf(this Task<Result> resultTask, Task<Result<bool>> conditionResult, Action action)
        {
            return conditionResult.Bind(condition => resultTask.TapIf(condition, action));
        }

        /// <summary>
        /// Negates the boolean payload of a successful Result&lt;bool&gt; asynchronously.
        /// </summary>
        /// <param name="result">Asynchronous Result&lt;bool&gt; to negate.</param>
        /// <returns>A Task of Result&lt;bool&gt; with inverted boolean.
        /// </returns>
        /// <remarks>
        /// Simplifies toggling boolean results in reactive or async contexts.
        /// </remarks>
        public static Task<Result<bool>> Not(this Task<Result<bool>> result)
        {
            return result.Map(b => !b);
        }

        /// <summary>
        /// Performs a side-effect <paramref name="action"/> if the asynchronous <paramref name="condition"/> evaluates to true.
        /// </summary>
        /// <param name="result">Base Result to continue if condition is true.</param>
        /// <param name="condition">Asynchronous boolean condition.</param>
        /// <param name="action">Synchronous side-effect to invoke.
        /// </param>
        /// <returns>A Task of Result reflecting side-effect execution.
        /// </returns>
        /// <remarks>
        /// Useful when non-Result boolean checks govern execution in a Result pipeline.
        /// </remarks>
        public static async Task<Result> TapIfB(this Result result, Task<bool> condition, Func<Task> func)
        {
            return await condition.ConfigureAwait(false) ? await result.Tap(func).ConfigureAwait(false) : await Task.FromResult(result).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes an asynchronous <paramref name="func"/> returning a payload of type <typeparamref name="T"/>
        /// if the asynchronous <paramref name="condition"/> is true, mapping into a Result&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T">Type of the payload produced by the side-effect.</typeparam>
        /// <param name="result">Base Result to continue if condition is true.</param>
        /// <param name="condition">Asynchronous boolean condition.</param>
        /// <param name="func">Asynchronous function returning T on success.
        /// </param>
        /// <returns>A Task of Result&lt;T&gt; with payload or original failure.
        /// </returns>
        /// <remarks>
        /// Supports conditional asynchronous side-effect that yields a new Result with content.
        /// </remarks>
        public static async Task<Result<T>> TapIf<T>(this Result<T> result, Task<bool> condition, Func<Task<T>> func)
        {
            return await condition.ConfigureAwait(false) ? await result.Tap(func).ConfigureAwait(false) : await Task.FromResult(result).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes an asynchronous side-effect <paramref name="func"/> if the asynchronous <paramref name="condition"/> is true,
        /// continuing a Task&lt;Result&gt; chain.
        /// </summary>
        /// <param name="resultTask">Task of Result to continue if condition is true.</param>
        /// <param name="condition">Asynchronous boolean condition.</param>
        /// <param name="func">Asynchronous side-effect to invoke.
        /// </param>
        /// <returns>A Task of Result reflecting side-effect execution or original error.
        /// </returns>
        /// <remarks>
        /// Use when conditional logic is needed within async Result workflows.
        /// </remarks>
        public static async Task<Result> TapIf(this Task<Result> resultTask, Task<bool> condition, Func<Task> func)
        {
            return await condition.ConfigureAwait(false) ? await resultTask.Tap(func).ConfigureAwait(false) : await resultTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Executes a synchronous side-effect <paramref name="action"/> if the asynchronous <paramref name="condition"/> is true,
        /// continuing a Task&lt;Result&gt; chain.
        /// </summary>
        /// <param name="resultTask">Task of Result to continue if condition is true.</param>
        /// <param name="condition">Asynchronous boolean condition.</param>
        /// <param name="action">Synchronous side-effect to invoke.
        /// </param>
        /// <returns>A Task of Result reflecting side-effect or original error.
        /// </returns>
        /// <remarks>
        /// Offers conditional synchronous side-effects within async Result pipelines.
        /// </remarks>
        public static async Task<Result> TapIf(this Task<Result> resultTask, Task<bool> condition, Action action)
        {
            return await condition.ConfigureAwait(false) ? await resultTask.Tap(action).ConfigureAwait(false) : await resultTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Applies an asynchronous side-effect <paramref name="func"/> on the payload of a Task&lt;Result&lt;T&gt;&gt;.
        /// </summary>
        /// <typeparam name="T">Type of the input Result payload.</typeparam>
        /// <param name="resultTask">Task of Result&lt;T&gt; to process.</param>
        /// <param name="func">Asynchronous function that consumes the payload.
        /// </param>
        /// <returns>A Task of Result indicating success or failure.
        /// </returns>
        /// <remarks>
        /// Facilitates performing asynchronous actions that do not alter the output but may have side-effects.
        /// </remarks>
        public static async Task<Result> Map<T>(this Task<Result<T>> resultTask, Func<T, Task> func)
        {
            return await (await resultTask.ConfigureAwait(false)).Map(func).ConfigureAwait(false);
        }

        /// <summary>
        /// Applies an asynchronous side-effect <paramref name="func"/> if the input Result&lt;T&gt; is successful.
        /// </summary>
        /// <typeparam name="T">Type of the input Result payload.</typeparam>
        /// <param name="result">Result&lt;T&gt; to process.</param>
        /// <param name="func">Asynchronous function that consumes the payload.
        /// </param>
        /// <returns>A Result indicating success or carrying the original error.
        /// </returns>
        /// <remarks>
        /// Preserves the functional style of applying effects only on successful Results.
        /// </remarks>
        public static async Task<Result> Map<T>(this Result<T> result, Func<T, Task> func)
        {
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            await func(result.Value).ConfigureAwait(false);
            return Result.Success();
        }

        /// <summary>
        /// Applies an asynchronous side-effect <paramref name="func"/> when the input Result succeeds.
        /// </summary>
        /// <param name="result">Result to process.</param>
        /// <param name="func">Asynchronous action to execute on success.
        /// </param>
        /// <returns>A Result indicating success or carrying the original error.
        /// </returns>
        /// <remarks>
        /// Use for executing generic asynchronous effects without payload.
        /// </remarks>
        public static async Task<Result> Map(this Result result, Func<Task> func)
        {
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            await func().ConfigureAwait(false);
            return Result.Success();
        }

        /// <summary>
        /// Transforms each element in the payload sequence of a successful Result&lt;IEnumerable&lt;TInput&gt;&gt; via <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TInput">Type of elements in the input sequence.</typeparam>
        /// <typeparam name="TResult">Type of elements produced by <paramref name="selector"/>.</typeparam>
        /// <param name="input">Result containing a sequence to transform.</param>
        /// <param name="selector">Mapping function to apply to each element.
        /// </param>
        /// <returns>A Result&lt;IEnumerable&lt;TResult&gt;&gt; on success or original failure.</returns>
        /// <remarks>
        /// Convenient for mapping over collections wrapped in a Result.
        /// </remarks>
        public static Result<IEnumerable<TResult>> MapEach<TInput, TResult>(this Result<IEnumerable<TInput>> input, Func<TInput, TResult> selector)
        {
            return input.Map(x => x.Select(selector));
        }

        /// <summary>
        /// Transforms each element in the payload sequence of a Maybe&lt;IEnumerable&lt;TInput&gt;&gt; via <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TInput">Type of elements in the input sequence.</typeparam>
        /// <typeparam name="TResult">Type of elements produced by <paramref name="selector"/>.
        /// </typeparam>
        /// <param name="input">Maybe containing a sequence to transform.</param>
        /// <param name="selector">Mapping function to apply to each element.
        /// </param>
        /// <returns>A Maybe&lt;IEnumerable&lt;TResult&gt;&gt; preserving the Maybe semantics.</returns>
        /// <remarks>
        /// Use when applying transformations inside optional sequences.
        /// </remarks>
        public static Maybe<IEnumerable<TResult>> MapEach<TInput, TResult>(this Maybe<IEnumerable<TInput>> input, Func<TInput, TResult> selector)
        {
            return input.Map(x => x.Select(selector));
        }

        /// <summary>
        /// Prepends an optional synchronous side-effect before an asynchronous Result task.
        /// </summary>
        /// <param name="result">Result wrapping a Task&lt;Result&gt; to invoke after <paramref name="prepend"/>.</param>
        /// <param name="prepend">Optional action to execute before the inner task.
        /// </param>
        /// <returns>A Task of Result after executing <paramref name="prepend"/> and the inner task.</returns>
        /// <remarks>
        /// Enables injecting synchronous logic prior to asynchronous Result computation.
        /// </remarks>
        public static Task<Result> Prepend(this Result<Task<Result>> result, Action? prepend = null)
        {
            return result.Bind(async task =>
            {
                prepend?.Invoke();
                return await task.ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Converts a source observable of <typeparamref name="T"/> into a source of Result&lt;T&gt; where each value is treated as success
        /// and exceptions are caught and mapped to failures with an optional <paramref name="errorMessage"/>.
        /// </summary>
        /// <typeparam name="T">Type of the source values.</typeparam>
        /// <param name="source">Sequence of values to wrap.</param>
        /// <param name="errorMessage">Optional error message for caught exceptions.</param>
        /// <returns>An observable of Result&lt;T&gt;.</returns>
        /// <remarks>
        /// Simplifies integration of legacy observables into a Result-based reactive pipeline.
        /// </remarks>
        public static IObservable<Result<T>> ToResult<T>(this IObservable<T> source, string? errorMessage = null)
        {
            return source.Select(Result.Success)
                .Catch<Result<T>, Exception>(ex =>
                    Observable.Return(Result.Failure<T>(errorMessage ?? ex.Message)));
        }

        /// <summary>
        /// Applies a timeout to a sequence of Result&lt;T&gt; values, converting timeouts into failures with an optional message.
        /// </summary>
        /// <typeparam name="T">Type of the Result payload.</typeparam>
        /// <param name="source">Source of Result&lt;T&gt; values.</param>
        /// <param name="timeout">Maximum duration to wait for a value.</param>
        /// <param name="timeoutMessage">Optional message for timeout failures.
        /// </param>
        /// <returns>An observable of Result&lt;T&gt; with timeout handling.</returns>
        /// <remarks>
        /// Essential for ensuring responsiveness in reactive sequences that may stall.
        /// </remarks>
        public static IObservable<Result<T>> WithTimeout<T>(this IObservable<Result<T>> source,
            TimeSpan timeout,
            string? timeoutMessage = null)
        {
            return source.Timeout(timeout)
                .Catch<Result<T>, TimeoutException>(_ =>
                    Observable.Return(Result.Failure<T>(timeoutMessage ?? "Operation timed out")));
        }
    }
}
