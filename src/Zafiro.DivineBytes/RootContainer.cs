namespace Zafiro.DivineBytes;

/// <summary>
/// Root container - represents the root of a file system or structure
/// No name required since it's the root
/// </summary>
public class RootContainer
{
    public IEnumerable<INamed> Children { get; }

    public RootContainer(IEnumerable<INamed> children)
    {
        Children = children.ToList();
    }
    
    public RootContainer(params INamed[] children)
        : this(children.AsEnumerable())
    {
    }

    public override string ToString()
    {
        return $"Root container ({Children.Count()} items)";
    }
}
