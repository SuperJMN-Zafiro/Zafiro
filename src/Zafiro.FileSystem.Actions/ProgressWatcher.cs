using System.Reactive.Linq;
using Zafiro.DataModel;

namespace Zafiro.FileSystem.Actions;

public class ProgressWatcher : IData, IDisposable
{
    private readonly IDisposable subscription;

    public ProgressWatcher(IData source, IObserver<long> progressObserver)
    {
        Source = source;
        ProgressObserver = progressObserver;

        long written = 0;
        var bytes = Source.Bytes
            .Do(bytes =>
            {
                written += bytes.Length;
                ProgressObserver.OnNext(written);
            });

        subscription = bytes.Subscribe(_ => { }, () => ProgressObserver.OnCompleted());
        Bytes = bytes;
    }

    public IData Source { get; }
    public IObserver<long> ProgressObserver { get; }

    public IObservable<byte[]> Bytes { get; }
    public long Length => Source.Length;

    public void Dispose()
    {
        subscription.Dispose();
    }
}