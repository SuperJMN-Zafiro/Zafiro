using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Core.Functional;
using Zafiro.Core.Mixins;

namespace Zafiro.UI.Transfers;

public abstract class TransferUnit : ITransfer
{
    private readonly ISubject<bool> canCancel = new Subject<bool>();

    protected TransferUnit(string name)
    {
        Name = name;

        Cancel = ReactiveCommand.Create(() => { }, canCancel);
        Start = ReactiveCommand.CreateFromObservable(() => Transfer().TakeUntil(Cancel));
        Start.IsExecuting.Subscribe(canCancel);
        ErrorMessage = Start.Failures();
        IsTransferring = Start.IsExecuting;
    }

    protected virtual string TransferText => "Transfer";
    protected virtual string ReTransferText => "Re-transfer";
    public IObservable<string> TransferButtonText => Start.Any().Select(_ => ReTransferText).StartWith(TransferText);
    public IObservable<bool> IsTransferring { get; }
    public IObservable<string> ErrorMessage { get; }
    public ReactiveCommand<Unit, Unit> Cancel { get; }
    public abstract IObservable<double> Progress { get; }
    public IObservable<TimeSpan> Eta => Progress.EstimatedCompletion();
    public ReactiveCommand<Unit, Result> Start { get; }
    public IObservable<bool> IsIndeterminate => Progress.Any().Not();
    public abstract TransferKey Key { get; }
    public string Name { get; }
    protected abstract IObservable<Result> Transfer();
}