using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.DataModel;
using Zafiro.Mixins;
using Zafiro.Reactive;

namespace Zafiro.FileSystem.Core;

public static class DataMixin
{
    public static IObservable<Result> ChunkedDump(this IData data, Stream stream, IScheduler? scheduler = null,
        CancellationToken cancellationToken = default)
    {
        return data.Bytes.DumpTo(stream, cancellationToken: cancellationToken, scheduler: scheduler);
    }

    public static Task<Result> DumpTo(this IData data, Stream stream, IScheduler? scheduler = null, CancellationToken cancellationToken = default)
    {
        return ChunkedDump(data, stream, cancellationToken: cancellationToken, scheduler: scheduler).ToList()
            .Select(list => list.Combine())
            .ToTask(cancellationToken);
    }

    public static async Task<Result> DumpTo(this IData data, string path, IScheduler? scheduler = null,
        CancellationToken cancellationToken = default)
    {
        using (var stream = File.Open(path, FileMode.Create))
        {
            return await data.DumpTo(stream, scheduler, cancellationToken);
        }
    }

    public static byte[] Bytes(this IData data)
    {
        return data.Bytes.ToEnumerable().Flatten().ToArray();
    }
}