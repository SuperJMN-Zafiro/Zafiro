using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.Core.IO;
using Zafiro.Core.Mixins;
using ObservableEx = Zafiro.Core.Mixins.ObservableEx;

namespace Zafiro.UI.Transfers;

public class RegularTransferUnit : TransferUnit
{
    private readonly Func<Task<Stream>> inputFactory;
    private readonly Func<Task<ProgressNotifyingStream>> outputFactory;
    private readonly Subject<TimeSpan> etaSubject = new ();
    private readonly Subject<double> progressSubject = new();

    public RegularTransferUnit(string name, Func<Task<Stream>> inputFactory, Func<Task<ProgressNotifyingStream>> outputFactory) : base(name)
    {
        this.inputFactory = inputFactory;
        this.outputFactory = outputFactory;
    }

    public override IObservable<double> Percent => progressSubject.AsObservable();
    public override IObservable<TimeSpan> Eta => progressSubject.EstimatedCompletion();

    protected override IObservable<Result> TransferObs()
    {
        return ObservableEx
            .Using(inputFactory, input => ObservableEx
                .Using(outputFactory, output =>
                {
                    return Observable
                        .Using(() => output.Progress.Subscribe(progressSubject), _ => Observable.FromAsync(ct => input.CopyToAsync(output, ct)))
                        .Select(_ => Result.Success())
                        .Catch((Exception ex) => Observable.Return(Result.Failure(ex.Message)));
                }));
    }

    public override string TransferText { get; }
    public override string ReTransferText { get; }
    public override IObservable<bool> IsIndeterminate { get; }
    public override TransferKey Key { get; }
}