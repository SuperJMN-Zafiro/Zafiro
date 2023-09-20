using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Zafiro.Actions;

public class CompositeAction : IAction
{
    private readonly IList<IAction> subActions;
    private readonly BehaviorSubject<IProportionProgress> progressSubject = new(new ProportionProgress());

    public CompositeAction(IList<IAction> subActions)
    {
        this.subActions = subActions;
    }

    public IObservable<IProportionProgress> Progress => progressSubject.AsObservable();

    public async Task<Result> Execute(CancellationToken ct)
    {
        var progressObservable = subActions.Select(x => x.Progress.Select(r => r.Proportion))
            .CombineLatest()
            .Select(list => list.Average())
            .TakeWhile(progress => progress < 1.0)
            .Concat(Observable.Return(1.0));

        progressObservable.Select(d => new ProportionProgress(d)).Subscribe(progressSubject);
        var tasks = subActions.Select(x => x.Execute(ct));
        var results = await Task.WhenAll(tasks).ConfigureAwait(false);
        return results.Combine();
    }
}