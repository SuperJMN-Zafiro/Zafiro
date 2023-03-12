#nullable enable
using CSharpFunctionalExtensions;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Zafiro.Core.IO;

public class ProgressNotifyingStream : Stream, IPositionable, IHaveProgress
{
    private readonly Stream inner;
    private readonly Subject<long> positionSubject = new();

    public ProgressNotifyingStream(Stream inner, Func<long>? getLength = default)
    {
        this.inner = inner;
        Progress = Positions.Select(x => (double)x / getLength?.Invoke() ?? Length);
    }

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

    public override bool CanRead => inner.CanRead;

    public override bool CanSeek => inner.CanSeek;

    public override bool CanWrite => inner.CanWrite;

    public override long Length => inner.Length;

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