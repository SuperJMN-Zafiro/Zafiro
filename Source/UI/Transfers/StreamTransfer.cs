using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace Zafiro.UI.Transfers;

public class StreamTransfer : ITransfer
{
    public StreamTransfer(string name, Func<Task<Stream>> originFactory, Func<Task<Stream>> destinationFactory)
    {
        Name = name;
        var copier = new StreamCopier(originFactory, destinationFactory);
        Start = copier.Start;
        Eta = copier.Eta;
        Percent = copier.Percent;
        Cancel = copier.Cancel;
        ErrorMessage = copier.ErrorMessage;
        IsTransferring = copier.Start.IsExecuting;

        TransferButtonText = copier.Start.Any().Select(_ => "Re-transfer").StartWith("Transfer");
        IsIndeterminate = copier.Percent
            .CombineLatest(copier.Start.IsExecuting, (percent, isExecuting) => percent == 0 && isExecuting);

        copier.Start.Execute().Subscribe();
    }

    public ReactiveCommand<Unit, Result> Start { get; }
    public IObservable<string> TransferButtonText { get; }
    public IObservable<string> ErrorMessage { get; }
    public string Name { get; }
    public ReactiveCommand<Unit, Unit> Cancel { get; }
    public IObservable<double> Percent { get; }
    public IObservable<TimeSpan> Eta { get; }
    public IObservable<bool> IsTransferring { get; }
    public IObservable<bool> IsIndeterminate { get; }
    public TransferKey Key => new(Name);
}