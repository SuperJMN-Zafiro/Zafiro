using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Zafiro.Core.Mixins
{
    public static class ObservableMixin
    {
        public static IObservable<TSource> Using<TSource, TResource>(
            Func<Task<TResource>> resourceFactoryAsync,
            Func<TResource, IObservable<TSource>> observableFactory)
            where TResource : IDisposable =>
            Observable.FromAsync(resourceFactoryAsync).SelectMany(
                resource => Observable.Using(() => resource, observableFactory));
    }
}