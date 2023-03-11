using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Core.Mixins;

namespace Zafiro.UI.Transfers;

public class SendTransfer : ITransfer
{
    private readonly Subject<double> progressSubject = new();
    private readonly Subject<bool> isIndeterminateSubject = new();

    public SendTransfer(string name, Func<Task<Stream>> input, Uri destination)
    {
        Name = name;
        Subject<bool> canStart = new();
        var start = ReactiveCommand.CreateFromTask(() => UploadWithProgress(input, destination, (sent, total) =>
        {
            if (total != null)
            {
                progressSubject.OnNext((double) sent / total.Value);
                isIndeterminateSubject.OnNext(false);
            }
            else
            {
                isIndeterminateSubject.OnNext(true);    
            }
        }), canStart);

        Start = start;
        Start.IsExecuting.Not().Subscribe(canStart);
        Cancel = ReactiveCommand.Create(() => { }, start.IsExecuting);
        IsTransferring = start.IsExecuting;
        Key = new TransferKey(Name);
        TransferButtonText = start.Any().Select(_ => "Re-download").StartWith("Download");
        Percent = progressSubject.AsObservable();
        Eta = progressSubject.Progress().Select(x => x.RemainingTime);
        IsIndeterminate = isIndeterminateSubject.DistinctUntilChanged().AsObservable();
        ErrorMessage = Start.WhereFailure();
    }

    private static async Task<Result> UploadWithProgress(Func<Task<Stream>> input, Uri destination, Action<long, long?> progressCallback, CancellationToken cancellationToken = default)
    {
        var httpClient = new HttpClient();
        var formData = new MultipartFormDataContent();
        var fileContent = new StreamContentWithProgress(await input(), 4096, progressCallback);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        formData.Add(fileContent, "file", "file.txt");
        var postAsync = await httpClient.PostAsync(destination, formData, cancellationToken);
        return Result.Try(() => postAsync.EnsureSuccessStatusCode());
    }

    public IObservable<string> ErrorMessage { get; }
    public string Name { get; }
    public ReactiveCommand<Unit, Unit> Cancel { get; }
    public IObservable<double> Percent { get; }
    public IObservable<TimeSpan> Eta { get; }
    public ReactiveCommand<Unit, Result> Start { get; }
    public IObservable<bool> IsTransferring { get; }
    public IObservable<bool> IsIndeterminate { get; }
    public TransferKey Key { get; }
    public IObservable<string> TransferButtonText { get; }

    private class StreamContentWithProgress : HttpContent
    {
        private readonly Stream stream;
        private readonly int chunkSize;
        private readonly Action<long, long?>? progressCallback;

        public StreamContentWithProgress(Stream stream, int chunkSize = 64*1024, Action<long, long?>? progressCallback = null)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkSize), "Chunk size must be greater than zero.");
            }

            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            this.chunkSize = chunkSize;
            this.progressCallback = progressCallback;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            var buffer = new byte[chunkSize];
            var bytesRead = 0L;
            var totalBytesRead = 0L;
            var contentLength = this.stream.Length;

            while (bytesRead >= 0 && (contentLength == null || totalBytesRead < contentLength))
            {
                bytesRead = await this.stream.ReadAsync(buffer, 0, chunkSize);
                await stream.WriteAsync(buffer, 0, (int)bytesRead);

                totalBytesRead += bytesRead;
                progressCallback?.Invoke(totalBytesRead, contentLength);
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = stream.Length;
            return true;
        }
    }

}


//using (var httpClient = new HttpClient())
//{
//    using (var formData = new MultipartFormDataContent())
//    {
//        // Crea una instancia de la clase StreamContentWithProgress y agrega el archivo adjunto a la solicitud
//        var fileContent = new StreamContentWithProgress(File.OpenRead(@"C:\path\to\file.txt"), 1024, (progress, total) => Console.WriteLine($"Progreso: {progress} de {total}"));

//        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
//        formData.Add(fileContent, "file", "file.txt");

//        // Envía la solicitud POST al servidor
//        var response = await httpClient.PostAsync("http://example.com/upload", formData);

//        // Lee la respuesta del servidor
//        var responseContent = await response.Content.ReadAsStringAsync();

//        // Procesa la respuesta del servidor
//        if (response.IsSuccessStatusCode)
//        {
//            // la solicitud fue exitosa
//            Console.WriteLine("Archivo subido exitosamente.");
//        }
//        else
//        {
//            // la solicitud falló
//            Console.WriteLine($"Error al subir archivo: {responseContent}");
//        }
//    }
//}
