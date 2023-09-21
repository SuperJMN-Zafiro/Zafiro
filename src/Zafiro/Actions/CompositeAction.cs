using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;

namespace Zafiro.Actions;

public class CompositeAction : IAction<LongProgress>
{
    private readonly IList<IAction<LongProgress>> subActions;
    private readonly BehaviorSubject<LongProgress> progressSubject = new(new LongProgress());

    public CompositeAction(IList<IAction<LongProgress>> subActions)
    {
        this.subActions = subActions;
    }

    public IObservable<LongProgress> Progress => progressSubject.AsObservable();

    public async Task<Result> Execute(CancellationToken ct)
    {
        var progressObservable = subActions
            .Select(x => x.Progress)
            .CombineLatest(GetProgress)
            .DistinctUntilChanged();
        
        progressObservable.Subscribe(progressSubject);
        var tasks = subActions.Select(x => x.Execute(ct));
        var results = await Task.WhenAll(tasks).ConfigureAwait(false);
        return results.Combine();
    }

    private static LongProgress GetProgress(IList<LongProgress> list)
    {
        var total = list.Select(progress => progress.Total).ToList().Combine(longs => longs.Sum());
        var current = list.Select(progress => progress.Current).ToList().Combine(longs => longs.Sum());
        var maybeProgress = from c in current
            from t in total
            select new LongProgress(c, t);
        return maybeProgress.GetValueOrDefault(new LongProgress());
    }
}