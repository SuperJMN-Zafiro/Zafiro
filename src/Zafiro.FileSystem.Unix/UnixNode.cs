using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Unix;

public abstract class UnixNode(string name) : INode
{
    public string Name { get; } = name;
}