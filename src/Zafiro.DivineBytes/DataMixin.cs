using CSharpFunctionalExtensions;
using Zafiro.Reactive;

namespace Zafiro.DivineBytes;

public static class DataMixin
{
    public static Stream ToStream(this IByteSource byteSource) => byteSource.Bytes.ToStream();
    public static IObservable<Result> DumpTo(this IByteSource byteSource, Stream destination) => byteSource.Bytes.DumpTo(destination);
}