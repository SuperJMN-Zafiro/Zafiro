namespace Zafiro.DivineBytes;

/// <summary>
/// Named container - always has a non-empty name
/// Used for subdirectories and named containers
/// </summary>
public class NamedContainer : IContainer
{
    public string Name { get; }
    public IEnumerable<IContainer> Subcontainers { get; }
    public IEnumerable<INamedByteSource> Resources { get; }

    public NamedContainer(string name, IEnumerable<INamedByteSource> resources, IEnumerable<IContainer> subcontainers)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Named container must have a non-empty name", nameof(name));
        
        Name = name;
        Resources = resources.ToList();
        Subcontainers = subcontainers.ToList();
    }
    
    public NamedContainer(string name, params INamed[] contents)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Named container must have a non-empty name", nameof(name));
        
        Name = name;
        Resources = contents.OfType<INamedByteSource>().ToList();
        Subcontainers = contents.OfType<IContainer>().ToList();
    }

    public override string ToString()
    {
        return $"Directory: {Name} ({Resources.Count()} files, {Subcontainers.Count()} subdirs)";
    }
}
