namespace Zafiro.DivineBytes;

public interface IDirectory : INode
{
    public IEnumerable<INode> Children { get; }
}