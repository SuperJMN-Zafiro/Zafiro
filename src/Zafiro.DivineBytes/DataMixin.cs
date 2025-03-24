using CSharpFunctionalExtensions;
using Zafiro.Reactive;

namespace Zafiro.DivineBytes;

public static class DataMixin
{
    public static Stream ToStream(this IData data) => data.Bytes.ToStream();
    public static IObservable<Result> DumpTo(this IData data, Stream destination) => data.Bytes.DumpTo(destination);
}