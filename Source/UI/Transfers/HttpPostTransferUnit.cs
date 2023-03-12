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
    private readonly Func<Task<Stream>> inputFactory;
    private readonly Func<HttpClient> clientFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly Uri uri;
    private readonly Subject<double> progressSubject = new();

    public HttpPostTransferUnit(string name, Func<Task<Stream>> inputFactory, Func<HttpClient> clientFactory, Uri uri) : base(name)
    {
        this.inputFactory = inputFactory;
        this.clientFactory = clientFactory;
        this.httpClientFactory = httpClientFactory;
        this.uri = uri;
    }

    public override IObservable<double> Percent => progressSubject.AsObservable();
    public override IObservable<TimeSpan> Eta { get; }
    protected override IObservable<Result> TransferObs()
    {
        async Task<Result> Post()
        {
            var output = new ProgressNotifyingStream(await inputFactory());
            using (output.Progress.Subscribe(progressSubject))
            {
                var multipartFormDataContent = new MultipartFormDataContent { new StreamContent(output) }; 
                var message = await clientFactory().PostAsync(uri, multipartFormDataContent);
                return Result.Try(() => message.EnsureSuccessStatusCode());
            }
        }

        return Observable.FromAsync(Post);
    }

    public override string TransferText { get; }
    public override string ReTransferText { get; }
    public override IObservable<bool> IsIndeterminate { get; }
    public override TransferKey Key { get; }
}