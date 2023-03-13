using System;
using System.IO;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.Core.IO;

namespace Zafiro.UI.Transfers;

public class HttpPostTransferUnit : TransferUnit
{
    private readonly Func<HttpClient> clientFactory;
    private readonly Func<Task<Stream>> inputFactory;
    private readonly Subject<double> progressSubject = new();
    private readonly Uri uri;

    public HttpPostTransferUnit(string name, Func<Task<Stream>> inputFactory, Func<HttpClient> clientFactory, Uri uri) : base(name)
    {
        this.inputFactory = inputFactory;
        this.clientFactory = clientFactory;
        this.uri = uri;
    }

    public override IObservable<double> Progress => progressSubject.AsObservable();
    protected override string TransferText => "Upload";
    protected override string ReTransferText => "Re-upload";
    public override TransferKey Key => new(Name);

    protected override IObservable<Result> Transfer()
    {
        async Task<Result> Post()
        {
            var output = new ProgressNotifyingStream(await inputFactory());
            using (output.Progress.Subscribe(progressSubject))
            {
                var multipartFormDataContent = new MultipartFormDataContent { new StreamContent(output) };
                var message = await clientFactory().PostAsync(uri, multipartFormDataContent);
                return Result.Try(message.EnsureSuccessStatusCode);
            }
        }

        return Observable.FromAsync(Post);
    }
}