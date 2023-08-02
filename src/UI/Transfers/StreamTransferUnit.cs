using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.Core.IO;
using ObservableEx = Zafiro.Core.Mixins.ObservableEx;

namespace Zafiro.UI.Transfers;

public class StreamTransferUnit : TransferUnit
{
    private readonly Func<Task<Stream>> inputFactory;
    private readonly Func<Stream, Task<ProgressNotifyingStream>> outputFactory;
    private readonly Subject<double> progressSubject = new();

    public StreamTransferUnit(string name, Func<Task<Stream>> inputFactory, Func<Stream, Task<ProgressNotifyingStream>> outputFactory) : base(name)
    {
        this.inputFactory = inputFactory;
        this.outputFactory = outputFactory;
    }

    public override IObservable<double> Progress => progressSubject.AsObservable();

    protected override string TransferText => "Download";
    protected override string ReTransferText => "Re-download";
    public override TransferKey Key => new(Name);

    protected override IObservable<Result> Transfer()
    {
        return ObservableEx
            .Using(inputFactory, input => ObservableEx
                .Using(() => outputFactory(input), output =>
                {
                    return Observable
                        .Using(() => output.Progress.Subscribe(progressSubject), _ => Observable.FromAsync(ct => Result.Try(() => input.CopyToAsync(output, ct))));
                }));
    }
}