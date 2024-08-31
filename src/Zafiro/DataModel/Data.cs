using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.Mixins;

namespace Zafiro.DataModel;

public class Data(IObservable<byte[]> bytes, long length) : IData
{
    public IObservable<byte[]> Bytes { get; } = bytes;
    public long Length { get; } = length;

    public static IData FromStream(Func<Stream> getStream, long length)
    {
        return new Data(Observable.Using(getStream, stream => stream.ToObservable()), length);
    }

    public static IData FromStream(Func<Task<Stream>> getStream, long length)
    {
        return new Data(Observable.FromAsync(getStream).SelectMany(stream =>
            Observable.Using(
                () => stream,
                s => s.ToObservable()
            )
        ), length);
    }

    private static Result<IData> FromStream(Func<Result<Stream>> getStream, long length)
    {
        return FromStream(() => Task.FromResult(getStream()), length);
    }

    public static Result<IData> FromStream(Func<Task<Result<Stream>>> getStream, long length)
    {
        return Result.Try(() =>
        {
            Func<Task<Stream>> asyncGetStream = async () =>
            {
                var streamResult = await getStream();
                return streamResult.Match(
                    stream => stream,
                    error => throw new Exception($"Error al obtener el stream: {error}")
                );
            };

            return FromStream(asyncGetStream, length);
        });
    }

    public static Result<IData> FromFileInfo(IFileInfo fileInfo)
    {
        return FromStream(() => Result.Try<Stream>(fileInfo.OpenRead), fileInfo.Length);
    }

    public static IData FromString(string content, Encoding? encoding = null)
    {
        return new Data(content.ToBytes(encoding ?? Encoding.Default).ToObservable().Buffer(1024).Select(list => list.ToArray()), content.Length);
    }

    public static IData FromByteArray(byte[] data)
    {
        return new Data(Observable.Return(data), data.Length);
    }
}