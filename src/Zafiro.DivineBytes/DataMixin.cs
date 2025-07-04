using CSharpFunctionalExtensions;
using Zafiro.Reactive;

namespace Zafiro.DivineBytes;

public static class DataMixin
{
    public static Stream ToStream(this IByteSource byteSource) => byteSource.Bytes.ToStream();
    public static IObservable<Result> WriteTo(this IByteSource byteSource, Stream destination) => byteSource.Bytes.WriteTo(destination);
    public static Task<Result> WriteTo(this IByteSource byteSource, string path)
    {
        return Result.Try(() => global::System.IO.File.WriteAllBytesAsync(path, byteSource.Array()));
    }
}