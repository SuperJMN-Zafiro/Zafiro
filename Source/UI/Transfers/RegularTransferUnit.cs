using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.Core.IO;
using ObservableEx = Zafiro.Core.Mixins.ObservableEx;

namespace Zafiro.UI.Transfers;

public class RegularTransferUnit : TransferUnit
{
    private readonly Subject<TimeSpan> etaSubject = new();
    private readonly Func<Task<Stream>> inputFactory;
    private readonly Func<Stream, Task<ProgressNotifyingStream>> outputFactory;
    private readonly Subject<double> progressSubject = new();

    public RegularTransferUnit(string name, Func<Task<Stream>> inputFactory, Func<Stream, Task<ProgressNotifyingStream>> outputFactory) : base(name)
    {
        this.inputFactory = inputFactory;
        this.outputFactory = outputFactory;
    }

    public override IObservable<double> Progress => progressSubject.AsObservable();

    protected override string TransferText { get; }
    protected override string ReTransferText { get; }
    public override TransferKey Key => new(Name);

    protected override IObservable<Result> Transfer()
    {
        return ObservableEx
            .Using(inputFactory, input => ObservableEx
                .Using(() => outputFactory(input), output =>
                {
                    return Observable
                        .Using(() => output.Progress.Subscribe(progressSubject), _ => Observable.FromAsync(ct => input.CopyToAsync(output, ct)))
                        .Select(_ => Result.Success())
                        .Catch((Exception ex) => Observable.Return(Result.Failure(ex.Message)));
                }));
    }
}