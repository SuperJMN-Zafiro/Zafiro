using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Optional;

namespace Zafiro.Core.Mixins;

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

    public static IObservable<byte> WriteTo(this IObservable<byte> bytes, Stream destination, int bufferSize = 4096)
    {
        return bytes
            .Buffer(bufferSize)
            .Select(buffer =>
            {
                return Observable.FromAsync(async ct =>
                {
                    await destination.WriteAsync(buffer.ToArray(), ct).AsTask();
                    return buffer;
                });
            })
            .Merge(1)
            .SelectMany(list => list);
    }

    //public static IObservable<double> UpdateProgressTo(this IObservable<double> observable, IObserver<ProgressSnapshot> observer)
    //{
    //    return observable.Publish(published =>
    //    {
    //        var progress = published
    //            .EstimatedCompletion()
    //            .OnErrorResumeNext(Observable.Never<TimeSpan>());

    //        var subscription = progress.Subscribe(observer);

    //        return published.Finally(() => subscription.Dispose());
    //    });
    //}
}