using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Zafiro.Core.IO;

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

    public override bool CanRead => this.inner.CanRead;

    public override bool CanSeek => this.inner.CanSeek;

    public override bool CanWrite => this.inner.CanWrite;

    public override long Length
    {
        get
        {
            try
            {
                this.lastKnownLength = new long?(this.inner.Length);
            }
            catch
            {
            }
            return this.lastKnownLength ?? long.MaxValue;
        }
    }

    public override long Position
    {
        get => this.inner.Position;
        set
        {
            this.inner.Position = value;
            this.positionSubject.OnNext(value);
        }
    }

    public IObservable<long> Positions => this.positionSubject.AsObservable<long>();

    public override void Flush() => this.inner.Flush();

    public override int Read(byte[] buffer, int offset, int count)
    {
        int num = this.inner.Read(buffer, offset, count);
        this.positionSubject.OnNext(this.Position);
        return num;
    }

    public override long Seek(long offset, SeekOrigin origin) => this.inner.Seek(offset, origin);

    public override void SetLength(long value) => this.inner.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
    {
        this.inner.Write(buffer, offset, count);
        this.positionSubject.OnNext(this.Position);
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        var readAsync = await base.ReadAsync(buffer, offset, count, cancellationToken);
        this.positionSubject.OnNext(this.Position);
        return readAsync;
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = new CancellationToken())
    {
        var readAsync = await base.ReadAsync(buffer, cancellationToken);
        this.positionSubject.OnNext(this.Position);
        return readAsync;
    }

    protected override void Dispose(bool disposing)
    {
        this.inner.Dispose();
        base.Dispose(disposing);
    }
}