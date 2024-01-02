using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Zafiro.Reactive;

[PublicAPI]
public class ObservableStream : Stream
{
    private readonly Stream inner;
    public override bool CanTimeout => inner.CanTimeout;
    private readonly Subject<long> positionSubject = new();
    private long? lastKnownLength;

    public ObservableStream(Stream inner) => this.inner = inner;
    public override int ReadTimeout
    {
        get => inner.ReadTimeout;
        set => inner.ReadTimeout = value;
    }

    public override bool CanRead => inner.CanRead;

    public override bool CanSeek => inner.CanSeek;

    public override bool CanWrite => inner.CanWrite;

    public override long Length
    {
        get
        {
            try
            {
                lastKnownLength = inner.Length;
            }
            catch
            {
            }
            return lastKnownLength ?? long.MaxValue;
        }
    }

    public override long Position
    {
        get => inner.Position;
        set
        {
            inner.Position = value;
            positionSubject.OnNext(value);
        }
    }

    public IObservable<long> Positions => positionSubject.AsObservable();

    public override void Flush() => inner.Flush();

    public override int Read(byte[] buffer, int offset, int count)
    {
        int num = inner.Read(buffer, offset, count);
        positionSubject.OnNext(Position);
        return num;
    }

    public override long Seek(long offset, SeekOrigin origin) => inner.Seek(offset, origin);

    public override void SetLength(long value) => inner.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
    {
        inner.Write(buffer, offset, count);
        positionSubject.OnNext(Position);
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        var readAsync = await base.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
        positionSubject.OnNext(Position);
        return readAsync;
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = new CancellationToken())
    {
        var readAsync = await base.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
        positionSubject.OnNext(Position);
        return readAsync;
    }

    protected override void Dispose(bool disposing)
    {
        inner.Dispose();
        base.Dispose(disposing);
    }

    public override ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return inner.DisposeAsync();
    }
}