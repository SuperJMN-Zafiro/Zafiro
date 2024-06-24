using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Zafiro.Actions;

public class CompositeAction : IAction<LongProgress>
{
    private readonly BehaviorSubject<LongProgress> progressSubject = new(new LongProgress());
    public IList<IAction<LongProgress>> Actions { get; }

    public CompositeAction(IList<IAction<LongProgress>> actions)
    {
        Actions = actions;
    }

    public int MaxConcurrency { get; set; } = 3;

    public IObservable<LongProgress> Progress => progressSubject.AsObservable();

    public Task<Result> Execute(CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<Result>();

        var progressObservable = Actions
            .Select(x => x.Progress)
            .CombineLatest(GetProgress)
            .DistinctUntilChanged();

        var subscription = progressObservable.Subscribe(progressSubject);

        var tasks = Actions
            .ToObservable()
            .Select(action =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return Observable.Return(Result.Failure("Cancelled"));
                }

                return Observable.FromAsync(() => action.Execute(cancellationToken));
            })
            .Merge(MaxConcurrency)
            .ToList();

        tasks.Subscribe(result =>
        {
            subscription.Dispose();
            tcs.SetResult(result.Combine());
        }, cancellationToken);

        return tcs.Task;
    }

    private static LongProgress GetProgress(IList<LongProgress> list)
    {
        return new LongProgress(list.Sum(x => x.Current), list.Sum(x => x.Total));
    }
}