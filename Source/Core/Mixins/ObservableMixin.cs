using System;
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
}