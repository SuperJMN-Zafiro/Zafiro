using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Refit;
using Zafiro.Core.IO;

namespace Zafiro.UI.Transfers;

public class RefitUploadUnit : TransferUnit
{
    private readonly Func<Task<Stream>> inputFactory;
    private readonly Func<StreamPart, CancellationToken, Task> upload;
    private readonly Subject<double> progressSubject = new();

    public RefitUploadUnit(string name, Func<Task<Stream>> inputFactory, Func<StreamPart, CancellationToken, Task> upload) : base(name)
    {
        this.inputFactory = inputFactory;
        this.upload = upload;
    }

    protected override string TransferText => "Upload";
    protected override string ReTransferText => "Re-upload";
    public override IObservable<double> Progress => progressSubject.AsObservable();
    public override TransferKey Key => new(Name);
    protected override IObservable<Result> Transfer()
    {
        async Task<Result> Post(CancellationToken ct)
        {
            var output = new ProgressNotifyingStream(await inputFactory());
            using (output.Progress.Subscribe(progressSubject))
            {
                var part = new StreamPart(output, Name);
                return await Result.Try(() => upload(part, ct));
            }
        }

        return Observable.FromAsync(Post);
    }
}