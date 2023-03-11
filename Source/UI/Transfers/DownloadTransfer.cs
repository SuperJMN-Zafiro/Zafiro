using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Zafiro.UI.Transfers;

public class DownloadTransfer : TransferBase
{
    private readonly StreamCopier copier;

    public DownloadTransfer(string name, Func<Task<Stream>> originFactory, Func<Task<Stream>> destinationFactory) : base(name)
    {
        copier = new StreamCopier(originFactory, destinationFactory);
        TransferText = "Download";
        ReTransferText = "Re-download";
        Percent = copier.Percent;
        Eta = copier.Eta;
        IsIndeterminate = copier.Percent
            .CombineLatest(copier.Start.IsExecuting, (percent, isExecuting) => percent == 0 && isExecuting);
    }

    public override IObservable<double> Percent { get; }
    public override IObservable<TimeSpan> Eta { get; }
    protected override IObservable<Result> TransferObs()
    {
        return copier.Obs;
    }

    public override string TransferText { get; }
    public override string ReTransferText { get; }
    public override IObservable<bool> IsIndeterminate { get; }
    public override TransferKey Key => new(Name);
}