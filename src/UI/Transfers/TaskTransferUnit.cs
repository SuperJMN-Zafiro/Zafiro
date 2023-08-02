#nullable enable
using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.Core.IO;

namespace Zafiro.UI.Transfers;

public class TaskTransferUnit : TransferUnit
{
    private readonly Func<Task<Stream>> contentStreamFactory;
    private readonly Func<Stream, CancellationToken, Task<Result>> transferFunc;
    private readonly Subject<double> progressSubject = new();

    public TaskTransferUnit(string name, Func<Task<Stream>> contentStreamFactory, Func<Stream, CancellationToken, Task<Result>> transferFunc) : base(name)
    {
        this.contentStreamFactory = contentStreamFactory;
        this.transferFunc = transferFunc;
    }

    protected override string TransferText => "Upload";
    protected override string ReTransferText => "Re-upload";
    public override IObservable<double> Progress => progressSubject.AsObservable();
    public override TransferKey Key => new(Name);
    protected override IObservable<Result> Transfer()
    {
        async Task<Result> TransferTask(CancellationToken ct)
        {
            var stream = await contentStreamFactory();

            try
            {
                var length = stream.Length;
            }
            catch
            {

            }

            using (var outputStream = new ProgressNotifyingStream(stream))
            {
                using (outputStream.Progress.Subscribe(progressSubject))
                {
                    return await transferFunc(outputStream, ct);
                }
            }
        }

        return Observable.FromAsync(TransferTask);
    }
}