using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Readonly;

public interface IDirectory : INode
{
    public IEnumerable<INode> Children { get; }
}