using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Zafiro.Actions;

namespace Zafiro.Reactive;

/// Thanks to Darrin Cullop for these methods.
public static class ProgressDoMixin
{
    /// <summary>
    ///     Like Do operator, but provides the total progress of the bytes pushed through <paramref name="bytes" />. Useful for reporting progress.
    /// </summary>
    public static IObservable<byte> ProgressDo(this IObservable<byte> bytes, Action<LongProgress> action, long length, TimeSpan bufferTime, IScheduler? scheduler = null)
    {
        return ProgressDo(bytes, current => action(new LongProgress(current, length)), bufferTime, scheduler);
    }

    /// <summary>
    ///     Like Do operator, but provides the number of the bytes pushed through <paramref name="bytes" />.
    /// </summary>
    public static IObservable<byte> ProgressDo(this IObservable<byte> bytes, Action<long> action, TimeSpan bufferTime, IScheduler? scheduler = null)
    {
        return Observable.Create<byte>(observer =>
        {
            var totalBytes = 0L;
            var shared = bytes.Publish();
            var subscribe = shared.Buffer(bufferTime, scheduler ?? Scheduler.Default)
                .Where(list => list.Count > 0) // Slightly more efficient than Any
                .Subscribe(list => action(totalBytes += list.Count), static err => { });

            return new CompositeDisposable(shared.SubscribeSafe(observer), subscribe, shared.Connect());
        });
    }
}