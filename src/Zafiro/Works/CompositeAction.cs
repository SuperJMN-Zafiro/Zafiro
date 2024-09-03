using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Zafiro.Progress;

namespace Zafiro.Works;

public class CompositeAction : IWork
{
    private readonly BehaviorSubject<IProgress> progressSubject = new(Unknown.Instance);

    public CompositeAction(IList<IWork> children)
    {
        Children = children;
    }

    public IList<IWork> Children { get; }

    public int MaxConcurrency { get; init; } = 3;

    public IObservable<IProgress> Progress => progressSubject.AsObservable();

    public IObservable<Result> Execute()
    {
        var progressObservable = Children
            .Select(x => x.Progress)
            .CombineLatest(GetProgress)
            .DistinctUntilChanged();

        return Observable.Using(() => progressObservable.Subscribe(progressSubject), _ => ExecuteChildren());
    }

    private IObservable<Result> ExecuteChildren()
    {
        return Children
            .ToObservable()
            .Select(work => work.Execute())
            .Merge(MaxConcurrency)
            .ToList()
            .Select(list => list.Combine());
    }

    private static IProgress GetProgress(IList<IProgress> list)
    {
        return new Unknown();
    }
}