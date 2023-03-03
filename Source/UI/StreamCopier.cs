using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Core;
using Zafiro.Core.Mixins;
using ObservableEx = Zafiro.Core.Mixins.ObservableEx;

namespace Zafiro.UI;

public class StreamCopier : IDisposable
{
    private readonly Subject<ProgressSnapshot> progressSubject = new();

    public StreamCopier(Func<Task<Stream>> origin, Func<Task<Stream>> destination)
    {
        var isExecuting = new Subject<bool>();

        Cancel = ReactiveCommand.Create(() => { }, isExecuting);
        Start = ReactiveCommand.CreateFromObservable(() =>
        {
            return ObservableEx.Using(origin, input => ObservableEx.Using(destination, output => Copy(input, output)))
                .TakeUntil(Cancel)
                .Catch((Exception ex) => Observable.Return(Result.Failure(ex.Message)));
        });

        Start.IsExecuting.Subscribe(isExecuting);
        ErrorMessage = Start.WhereFailure();
    }

    public ReactiveCommand<Unit, Unit> Cancel { get; }

    public IObservable<string> ErrorMessage { get; }

    private IObservable<Result> Copy(Stream input, Stream output)
    {
        return input
            .ToObservableCustom()
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
        .Select(progressSnapshot => progressSnapshot.CurrentProgress)
        .StartWith(0d);

    public ReactiveCommand<Unit, Result> Start { get; }

    public void Dispose()
    {
        progressSubject?.Dispose();
        Cancel?.Dispose();
        Start?.Dispose();
    }
}
