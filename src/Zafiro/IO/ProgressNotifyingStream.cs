using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Zafiro.IO;

[Obsolete("Use ObservableStream")]
public class ProgressNotifyingStream : Stream
{
    private readonly Stream inner;
    private readonly Func<long>? getLength;
    private readonly Subject<long> positionSubject = new();
    private long? lastKnownLength;

    public ProgressNotifyingStream(Stream inner, Func<long>? getLength = default)
    {
        this.inner = inner;
        this.getLength = getLength;
        Progress = Positions.Select(x => (double)x / Length);
    }

    public override void Flush()
    {
        inner.Flush();
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

    public override int Read(byte[] buffer, int offset, int count)
    {
        var read = inner.Read(buffer, offset, count);
        positionSubject.OnNext(Position);
        return read;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return inner.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        inner.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        inner.Write(buffer, offset, count);
        positionSubject.OnNext(Position);
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
                if (getLength != null)
                {
                    lastKnownLength = getLength();
                }

                lastKnownLength = inner.Length;
            }
            catch
            {
                // We save the last known length because Android tends to close the stream (channel) and unable to report the Length when this happens.
                // It throws an exception instead. That's why report the last known length anyways.
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
    public IObservable<double> Progress { get; }
}