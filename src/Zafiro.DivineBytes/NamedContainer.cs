namespace Zafiro.DivineBytes;

/// <summary>
/// Named container - always has a non-empty name
/// Used for subdirectories and named containers
/// </summary>
public class NamedContainer : IContainer
{
    public string Name { get; }
    public IEnumerable<INamed> Children { get; }

    public NamedContainer(string name, IEnumerable<INamed> children)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Named container must have a non-empty name", nameof(name));
        
        Name = name;
        Children = children.ToList();
    }
    
    public NamedContainer(string name, params INamed[] children)
        : this(name, children.AsEnumerable())
    {
    }

    public override string ToString()
    {
        return $"Directory: {Name} ({Children.Count()} items)";
    }
}
