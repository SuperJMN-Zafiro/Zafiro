namespace Zafiro.DivineBytes;

/// <summary>
/// Root container - represents the root of a file system or structure
/// No name required since it's the root
/// </summary>
public class RootContainer : IContainer
{
    public IEnumerable<INamedContainer> Subcontainers { get; }
    public IEnumerable<INamedByteSource> Resources { get; }

    public RootContainer(IEnumerable<INamedByteSource> resources, IEnumerable<INamedContainer> subcontainers)
    {
        Resources = resources.ToList();
        Subcontainers = subcontainers.ToList();
    }

    public override string ToString()
    {
        return $"Root container ({Resources.Count()} files, {Subcontainers.Count()} subdirs)";
    }
}
