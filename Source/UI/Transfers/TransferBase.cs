using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Core.Mixins;

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