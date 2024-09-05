using System.Reactive.Concurrency;
using CSharpFunctionalExtensions;
using Zafiro.DataModel;

namespace Zafiro.FileSystem.Mutable;

public interface IMutableFile : IMutableNode
{
    Task<Result> SetContents(IData data, IScheduler? scheduler = null, CancellationToken cancellationToken = default);
    Task<Result<IData>> GetContents();
}