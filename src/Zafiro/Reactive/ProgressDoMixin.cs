using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Zafiro.Actions;

namespace Zafiro.Reactive;

public static class ProgressDoMixin
{
    public static IObservable<byte> ProgressDo(this IObservable<byte> bytes, long length, Action<LongProgress> action, IScheduler? scheduler = null)
    {
        long totalBytes = 0;

        return bytes
            .Buffer(TimeSpan.FromSeconds(1), scheduler: scheduler ?? Scheduler.Default)
            .Where(list => list.Any())
            .Do(list =>
            {
                totalBytes += list.Count; 
                var progress = new LongProgress(totalBytes, length);
                action(progress);
            })
            .SelectMany(listOfBytes => listOfBytes);
    }

    public static IObservable<byte> ProgressDo(this IObservable<byte> bytes, TimeSpan bufferTime, Action<long> action, IScheduler? scheduler = null) => Observable.Create<byte>(observer =>
    {
        var totalBytes = 0L;
        var shared = bytes.Publish();
        var subscribe = shared.Buffer(bufferTime, scheduler: scheduler ?? Scheduler.Default)
            .Where(list => list.Count > 0)  // Slightly more efficient than Any
            .Subscribe(list => action(totalBytes += list.Count));

        return new CompositeDisposable(shared.SubscribeSafe(observer), subscribe, shared.Connect());
    });
}