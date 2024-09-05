using System.Reactive.Linq;

namespace Zafiro.FileSystem.Core;

public class ReadTimeOutStream : Stream
{
    private readonly Stream inner;

    public ReadTimeOutStream(Stream stream)
    {
        inner = stream;
    }

    public override int ReadTimeout { get; set; }

    public override bool CanTimeout => true;

    public override bool CanRead => inner.CanRead;

    public override bool CanSeek => inner.CanSeek;

    public override bool CanWrite => inner.CanWrite;

    public override long Length => inner.Length;

    public override long Position
    {
        get => inner.Position;
        set => inner.Position = value;
    }

    public override void Flush()
    {
        inner.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (ReadTimeout == 0)
        {
            return inner.Read(buffer, offset, count);
        }

        return Observable.Start(() => inner.Read(buffer, offset, count)).Timeout(TimeSpan.FromMilliseconds(ReadTimeout)).Wait();
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