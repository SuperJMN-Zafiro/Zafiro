using System;
using System.Reactive;
using System.Windows.Input;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace Zafiro.UI.Transfers;

public interface ITransfer
{
    IObservable<string> ErrorMessage { get; }
    string Name { get; }
    ReactiveCommand<Unit, Unit> Cancel { get; }
    IObservable<double> Percent { get; }
    IObservable<TimeSpan> Eta { get; }
    ReactiveCommand<Unit, Result> Start { get; }
    IObservable<bool> IsTransferring { get; }
    IObservable<bool> IsIndeterminate { get; }
    TransferKey Key { get; }
    IObservable<string> TransferButtonText { get; }
}