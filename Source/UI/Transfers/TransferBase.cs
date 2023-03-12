using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Core.IO;
using Zafiro.Core.Mixins;
using ObservableEx = Zafiro.Core.Mixins.ObservableEx;

namespace Zafiro.UI.Transfers;

public abstract class TransferBase : ITransfer
{
    private readonly ISubject<bool> canCancel = new Subject<bool>();

    public TransferBase(string name)
    {
        Name = name;

        Cancel = ReactiveCommand.Create(() => { }, canCancel);
        Start = ReactiveCommand.CreateFromObservable(() => TransferObs().TakeUntil(Cancel));
        Start.IsExecuting.Subscribe(canCancel);
        ErrorMessage = Start.WhereFailure();
        IsTransferring = Start.IsExecuting;
        TransferButtonText = Start.Any().Select(_ => "Re-transfer").StartWith("Transfer");
    }

    public IObservable<string> TransferButtonText { get; }

    public IObservable<bool> IsTransferring { get; }

    public IObservable<string> ErrorMessage { get; }

    public ReactiveCommand<Unit, Unit> Cancel { get; }
    public abstract IObservable<double> Percent { get; }
    public abstract IObservable<TimeSpan> Eta { get; }

    public ReactiveCommand<Unit, Result> Start { get; }

    protected abstract IObservable<Result> TransferObs();
    
    public abstract string TransferText { get; }
    public abstract string ReTransferText { get; }
    public abstract IObservable<bool> IsIndeterminate { get; }
    public abstract TransferKey Key { get; }
    public string Name { get; }
}

public class TransferUnit : TransferBase
{
    private readonly Func<Task<Stream>> inputFactory;
    private readonly Func<Task<ProgressNotifyingStream>> outputFactory;
    private readonly Subject<TimeSpan> etaSubject = new ();
    private readonly Subject<double> progressSubject = new();

    public TransferUnit(string name, Func<Task<Stream>> inputFactory, Func<Task<ProgressNotifyingStream>> outputFactory) : base(name)
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