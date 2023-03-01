using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Core;
using Zafiro.Core.Mixins;

namespace Zafiro.UI;

public class DownloadUnit
{
    private readonly Subject<ProgressSnapshot> progressSubject = new();

    public DownloadUnit(Func<FileStream> origin, Func<FileStream> destination)
    {
        var isExecuting = new Subject<bool>();

        Cancel = ReactiveCommand.Create(() => { }, isExecuting);
        Start = ReactiveCommand.CreateFromObservable(() =>
        {
            return Observable.Using(origin, input => Observable.Using(destination, output => Copy(input, output)))
                .TakeUntil(Cancel);
        });

        Start.IsExecuting.Subscribe(isExecuting);
        ErrorMessage = Start.WhereFailure();
    }

    public ReactiveCommand<Unit, Unit> Cancel { get; set; }

    public IObservable<string> ErrorMessage { get; }

    private IObservable<Result> Copy(Stream input, Stream output)
    {
        return input
            .ToObservable()
            .WriteTo(output)
            .Select(written => (double) written / input.Length)
            .UpdateProgressTo(progressSubject)
            .LastAsync()
            .Select(_ => Result.Success())
            .Catch((Exception ex) => Observable.Return(Result.Failure(ex.Message)));
    }

    public IObservable<TimeSpan> Eta => progressSubject
        .CombineLatest(Start.IsExecuting)
        .Select(span => span.Second ? Calculate.Max(TimeSpan.Zero, span.First.RemainingTime) : TimeSpan.MaxValue)
        .AsObservable();

    public IObservable<double> Percent => progressSubject
        .CombineLatest(Start.IsExecuting)
        .Select(tuple => tuple.Second ? tuple.First.CurrentProgress : 0d).AsObservable();

    public ReactiveCommand<Unit, Result> Start { get; }
}
