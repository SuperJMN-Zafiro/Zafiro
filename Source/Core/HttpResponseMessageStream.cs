using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Core.Tests;

public class HttpResponseMessageStream : Stream
{
    private readonly HttpResponseMessage response;

    private readonly Stream inner;

    private HttpResponseMessageStream(Stream stream, HttpResponseMessage response)
    {
        inner = stream;
        this.response = response;
    }

    public override bool CanRead => inner.CanRead;

    public override bool CanSeek => inner.CanSeek;

    public override bool CanWrite => inner.CanWrite;

    public override long Length => inner.Length;

    public override long Position
    {
        get => inner.Position;
        set => inner.Position = value;
    }

    public static async Task<HttpResponseMessageStream> Create(HttpResponseMessage response)
    {
        return new HttpResponseMessageStream(await response.Content.ReadAsStreamAsync(), response);
    }

    public override ValueTask DisposeAsync()
    {
        response.Dispose();
        return base.DisposeAsync();
    }

    public override void Flush()
    {
        inner.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return inner.Read(buffer, offset, count);
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
        response.Dispose();
        base.Dispose(disposing);
    }
}