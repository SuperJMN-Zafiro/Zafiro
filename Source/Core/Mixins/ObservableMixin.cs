using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Zafiro.Core.Mixins;

public static class ObservableMixin
{
    public static IObservable<TSource> Using<TSource, TResource>(
        Func<Task<TResource>> resourceFactoryAsync,
        Func<TResource, IObservable<TSource>> observableFactory)
        where TResource : IDisposable
    {
        return Observable.FromAsync(resourceFactoryAsync).SelectMany(
            resource => Observable.Using(() => resource, observableFactory));
    }

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

    public static IObservable<T> WhereSuccess<T>(this IObservable<Result<T>> self)
    {
        return self.Where(a => a.IsSuccess)
            .Select(x => x.Value);
    }

    public static IObservable<bool> IsSuccess<T>(this IObservable<Result<T>> self)
    {
        return self
            .Select(a => a.IsSuccess);
    }

    public static IObservable<string> WhereNotEmpty(this IObservable<string> self)
    {
        return self.Where(s => !string.IsNullOrWhiteSpace(s));
    }

    public static IObservable<bool> SelectNotEmpty(this IObservable<string> self)
    {
        return self.Select(s => !string.IsNullOrWhiteSpace(s));
    }

    public static IObservable<ProgressEstimation> EstimatedRemainingTime(this IObservable<double> progress)
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
            .Select(x => new ProgressEstimation(x.current, DateTimeOffset.Now.AddSeconds((1.0 - x.current) / x.rate), TimeSpan.FromSeconds(1.0 - x.current) / x.rate));		
    }
}