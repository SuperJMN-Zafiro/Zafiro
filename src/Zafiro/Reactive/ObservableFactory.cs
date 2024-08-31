using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Zafiro.Reactive;

public static class ObservableFactory
{
    public static IObservable<TSource> UsingAsync<TSource, TResource>(
        Func<Task<TResource>> resourceFactoryAsync,
        Func<TResource, IObservable<TSource>> observableFactory)
        where TResource : IDisposable
    {
        return Observable.FromAsync(resourceFactoryAsync).SelectMany(
            resource => Observable.Using(() => resource, observableFactory));
    }
}