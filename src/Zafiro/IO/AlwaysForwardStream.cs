using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Zafiro.IO;

[PublicAPI]
public class AlwaysForwardStream : Stream
{
    private readonly Stream inner;
    private long position;

    public AlwaysForwardStream(Stream inner, long length)
    {
        this.inner = inner;
        this.Length = length;
    }

    public override bool CanRead => inner.CanRead;

    public override bool CanSeek => inner.CanSeek;

    public override bool CanWrite => inner.CanWrite;

    public override long Length { get; }

    public override long Position
    {
        get => position;
        set => inner.Position = value;
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
        position += read;
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
        position += count;
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        var readAsync = base.ReadAsync(buffer, offset, count, cancellationToken);
        position += count;
        return readAsync;
    }

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        await base.WriteAsync(buffer, offset, count, cancellationToken);
        position += count;
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = new())
    {
        var readAsync = await base.ReadAsync(buffer, cancellationToken);
        position += readAsync;
        return readAsync;
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = new())
    {
        await base.WriteAsync(buffer, cancellationToken);
        position += buffer.Length;
    }
}