using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace Zafiro.Reactive;

public static class ObservableMixin
{
    public static IObservable<Unit> ToSignal<T>(this IObservable<T> source)
    {
        return source.Select(_ => Unit.Default);
    }

    public static IObservable<T> ReplayLastActive<T>(this IObservable<T> observable)
    {
        return observable.Replay(1).RefCount();
    }

    public static IObservable<bool> Not(this IObservable<bool> self)
    {
        return self.Select(b => !b);
    }

    public static IObservable<bool> Null<T>(this IObservable<T?> self)
    {
        return self.Select(b => b is null);
    }

    public static IObservable<bool> NotNull<T>(this IObservable<T?> self)
    {
        return self.Select(b => b is not null);
    }

    public static IObservable<bool> NullOrWhitespace(this IObservable<string> self)
    {
        return self.Select(string.IsNullOrWhiteSpace);
    }

    public static IObservable<bool> NotNullOrEmpty(this IObservable<string> self)
    {
        return self.Select(s => !string.IsNullOrWhiteSpace(s));
    }

    public static IObservable<bool> Whitespace(this IObservable<string> self)
    {
        return self.Select(s => !s.Any(char.IsWhiteSpace));
    }

    public static IObservable<double> Rate(this IObservable<long> progress)
    {
        return progress
            .Timestamp()
            .Buffer(2, 1)
            .Where(tsProgresses => tsProgresses.Count == 2)
            .Scan((a, b) => a.Take(1).Concat(b).Take(1).Concat(b.Skip(1)).ToList())
            .Select(x => new
            {
                current = x[1].Value,
                delta = x[1].Timestamp.Subtract(x[0].Timestamp),
            })
            .Select(x => x.current / x.delta.TotalSeconds);
    }

    public static IObservable<TimeSpan> EstimatedCompletion(this IObservable<double> progress)
    {
        return progress
            .Timestamp()
            .Buffer(2, 1)
            .Where(tsProgresses => tsProgresses.Count == 2)
            .Scan((a, b) => a.Take(1).Concat(b).Take(1).Concat(b.Skip(1)).ToList())
            .Select(x => new
            {
                current = x[1].Value,
                delta = x[1].Timestamp.Subtract(x[0].Timestamp),
            })
            .Select(x => new
            {
                x.current,
                rate = x.current / x.delta.TotalSeconds,
            })
            .Select(x => TimeSpan.FromSeconds(1.0 - x.current) / x.rate);
    }

    // The Retry With Backoff code is created by: https://gist.github.com/niik
    // Find the original code here: https://gist.github.com/niik/6696449
    // Licensed under the MIT license with <3 by GitHub

    /// <summary>
    /// An exponential back off strategy which starts with 1 second and then 4, 9, 16...
    /// </summary>
    public static readonly Func<int, TimeSpan> ExponentialBackoff = n => TimeSpan.FromSeconds(Math.Pow(n, 2));

    /// <summary>
    /// Returns a cold observable which retries (re-subscribes to) the source observable on error up to the 
    /// specified number of times or until it successfully terminates. Allows for customizable back off strategy.
    /// </summary>
    /// <param name="source">The source observable.</param>
    /// <param name="retryCount">The number of attempts of running the source observable before failing.</param>
    /// <param name="strategy">The strategy to use in backing off, exponential by default.</param>
    /// <param name="retryOnError">A predicate determining for which exceptions to retry. Defaults to all</param>
    /// <param name="scheduler">The scheduler.</param>
    /// <returns>
    /// A cold observable which retries (re-subscribes to) the source observable on error up to the 
    /// specified number of times or until it successfully terminates.
    /// </returns>
    public static IObservable<T> RetryWithBackoffStrategy<T>(
        this IObservable<T> source,
        int retryCount = 3,
        Func<int, TimeSpan> strategy = null,
        Func<Exception, bool> retryOnError = null,
        IScheduler scheduler = null)
    {
        strategy ??= ExponentialBackoff;
        scheduler ??= RxApp.TaskpoolScheduler;

        if (retryOnError == null)
        {
            retryOnError = _ => true;
        }

        int attempt = 0;

        return Observable.Defer(() =>
        {
            return (++attempt == 1 ? source : source.DelaySubscription(strategy(attempt - 1), scheduler))
                .Select(Notification.CreateOnNext)
                .Catch((Exception e) => retryOnError(e)
                    ? Observable.Throw<Notification<T>>(e)
                    : Observable.Return(Notification.CreateOnError<T>(e)));
        })
        .Retry(retryCount)
        .Dematerialize();
    }

    public static Stream ToStream(this IObservable<byte> observable, int bufferSize = 4096, int maxConcurrency = 3)
    {
        var pipe = new System.IO.Pipelines.Pipe();

        var reader = pipe.Reader;
        var writer = pipe.Writer;

        observable
            .Buffer(bufferSize)
            .Select(buffer => Observable.FromAsync(async () => await writer.WriteAsync(buffer.ToArray())))
            .Concat()
            .Subscribe(_ => { }, onCompleted: () => { writer.Complete(); }, onError: exception => writer.Complete(exception));

        return reader.AsStream();
    }

    /// <summary>
    /// Thanks to Darrin Cullop.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="parent"></param>
    /// <param name="selector"></param>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static IDisposable UpdateCollectionWhenSomeOtherCollectionObservableChanges<T, TItem>(
        this T parent,
        Expression<Func<T, ReadOnlyObservableCollection<TItem>>> selector, out ReadOnlyObservableCollection<TItem> collection) where TItem : notnull where T : ReactiveObject
    {
        CompositeDisposable disposable = new();
        var source = new SourceList<TItem>()
            .DisposeWith(disposable);

        parent.WhenAnyValue(selector)
            .Do(r => source.EditDiff(r))
            .Select(r => r.ToObservableChangeSet())
            .Switch()
            .PopulateInto(source)
            .DisposeWith(disposable);

        source.Connect()
            .Bind(out collection)
            .Subscribe()
            .DisposeWith(disposable);

        return disposable;
    }

    /// <summary>
    /// Makes a sequence of observable collection a single observable collection. The most recent observable collection will populate the <paramref name="collection"/> and will follow changes happening to it.
    /// Thanks to Darrin Cullop.
    /// </summary>
    /// <typeparam name="T">Type of the items</typeparam>
    /// <param name="observableOfCollections">Sequence of observable collections</param>
    /// <param name="collection">The output collection</param>
    /// <returns></returns>
    public static IDisposable Bind<T>(
        this IObservable<ReadOnlyObservableCollection<T>> observableOfCollections, out ReadOnlyObservableCollection<T> collection)
    {
        CompositeDisposable disposable = new();
        var source = new SourceList<T>()
            .DisposeWith(disposable);

        observableOfCollections
            .Do(r => source.EditDiff(r))
            .Select(obsCollection => obsCollection.ToObservableChangeSet())
            .Switch()
            .PopulateInto(source)
            .DisposeWith(disposable);

        source.Connect()
            .Bind(out collection)
            .Subscribe()
            .DisposeWith(disposable);

        return disposable;
    }
}