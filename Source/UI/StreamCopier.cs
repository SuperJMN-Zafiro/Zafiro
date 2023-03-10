using System;
using System.IO;
using System.Linq;
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
            .ToObservable()
            .Publish(inputBytes =>
            {
                var writer = inputBytes.WriteTo(output).Subscribe();
                var progressUpdater = inputBytes.Select((_, i) => i).Select(x => x / (double) input.Length)
                    .Buffer(TimeSpan.FromSeconds(0.2))
                    .Select(list => list.TryLast())
                    .Values()
                    .UpdateProgressTo(progressSubject).Subscribe();

                return inputBytes.Finally(() =>
                {
                    writer.Dispose();
                    progressUpdater.Dispose();
                });
            })
            .LastAsync()
            .Select(_ => Result.Success());
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
