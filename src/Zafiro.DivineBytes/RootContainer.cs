namespace Zafiro.DivineBytes;

/// <summary>
/// Root container - represents the root of a file system or structure
/// No name required since it's the root
/// </summary>
public class RootContainer
{
    public IEnumerable<IContainer> Subcontainers { get; }
    public IEnumerable<INamedByteSource> Resources { get; }

    public RootContainer(IEnumerable<INamedByteSource> resources, IEnumerable<IContainer> subcontainers)
    {
        Resources = resources.ToList();
        Subcontainers = subcontainers.ToList();
    }
    
    public RootContainer(params INamed[] contents)
    {
        Resources = contents.OfType<INamedByteSource>().ToList();
        Subcontainers = contents.OfType<IContainer>().ToList();
    }

    public override string ToString()
    {
        return $"Root container ({Resources.Count()} files, {Subcontainers.Count()} subdirs)";
    }
}
