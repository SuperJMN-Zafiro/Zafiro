using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Zafiro.FileSystem.Core;

public class PositionReportingStream : Stream, IObservable<long>
{
    private readonly BehaviorSubject<long> positionSubject;
    private readonly Stream source;

    public PositionReportingStream(Stream source)
    {
        this.source = source;
        positionSubject = new BehaviorSubject<long>(source.Position);
    }

    public IObservable<long> Positions => positionSubject.AsObservable();

    public override bool CanRead => source.CanRead;

    public override bool CanSeek => source.CanSeek;

    public override bool CanWrite => source.CanWrite;

    public override long Length => source.Length;

    public override long Position
    {
        get => source.Position;
        set
        {
            source.Position = value;
            positionSubject.OnNext(value);
        }
    }

    public IDisposable Subscribe(IObserver<long> observer)
    {
        return positionSubject.Subscribe(observer);
    }

    public override void Flush()
    {
        source.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var result = source.Read(buffer, offset, count);
        positionSubject.OnNext(source.Position);
        return result;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        var result = source.Seek(offset, origin);
        positionSubject.OnNext(source.Position);
        return result;
    }

    public override void SetLength(long value)
    {
        source.SetLength(value);
        positionSubject.OnNext(source.Position);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        source.Write(buffer, offset, count);
        positionSubject.OnNext(source.Position);
    }
}