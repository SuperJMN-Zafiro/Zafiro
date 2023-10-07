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
    private readonly IList<IAction<LongProgress>> actions;

    public CompositeAction(IList<IAction<LongProgress>> actions)
    {
        this.actions = actions;
    }

    public int MaxConcurrency { get; set; } = 3;

    public IObservable<LongProgress> Progress => progressSubject.AsObservable();

    public async Task<Result> Execute(CancellationToken cancellationToken)
    {
        var progressObservable = actions
            .Select(x => x.Progress)
            .CombineLatest(GetProgress)
            .DistinctUntilChanged();

        using (_ = progressObservable.Subscribe(progressSubject))
        {
            var tasks = actions
                .ToObservable()
                .Select(action => Observable.FromAsync(() => action.Execute(cancellationToken)))
                .Merge(MaxConcurrency)
                .ToList();

            var results = await tasks;
            return results.Combine();
        }
    }

    private static LongProgress GetProgress(IList<LongProgress> list)
    {
        return new LongProgress(list.Sum(x => x.Current), list.Sum(x => x.Total));
    }
}