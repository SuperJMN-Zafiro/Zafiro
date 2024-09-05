using CSharpFunctionalExtensions;
using Zafiro.Actions;

namespace Zafiro.FileSystem.Core;

public interface ISyncAction
{
    public IObservable<IProgress> Progress { get; }
    public Task<Result> Sync(CancellationToken cancellationToken);
}