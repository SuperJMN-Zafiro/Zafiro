using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Core;

public interface IAsyncDir : INode
{
    public Task<Result<IEnumerable<INode>>> Children();
}