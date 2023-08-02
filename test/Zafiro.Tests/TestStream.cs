using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace Zafiro.Tests;

public class TestStream : Stream
{
    private readonly (byte[], TimeSpan)[] datas;
    private readonly IScheduler scheduler;
    private int read; 

    public TestStream((byte[], TimeSpan)[] datas, IScheduler scheduler)
    {
        this.scheduler = scheduler;
        this.datas = datas;
        read = 0; 
    }

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length { get; }
    public override long Position { get; set; }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (read == datas.Length)
        {
            return 0;
        }

        datas[read].Item1.CopyTo(buffer, offset);
        scheduler.Sleep(datas[read].Item2);
        var readBytes = datas[read].Item1.Length;
        read++;
        return readBytes;
    }


    // Other methods remain unchanged...
    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (read == datas.Length)
        {
            return 0;
        }

        datas[read].Item1.CopyTo(buffer, offset);
        await scheduler.Sleep(datas[read].Item2);
        var readBytes = datas[read].Item1.Length;
        read++;
        return readBytes;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}