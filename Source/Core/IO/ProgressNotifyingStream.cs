using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Zafiro.Core.IO;

public class ObservableStream : Stream
{
    private readonly Stream inner;
    private readonly Subject<long> positionSubject = new();
    private long? lastKnownLength;

    public ObservableStream(Stream inner)
    {
        this.inner = inner;
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
                // We save the last known length because Android tends to close the stream (channel) and unable to report the Length when this happens.
                // It throws an exception instead. That's why we report the last known length anyways.
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

    public override void Flush()
    {
        inner.Flush();
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

    protected override void Dispose(bool disposing)
    {
        inner.Dispose();
        base.Dispose(disposing);
    }
}