using CSharpFunctionalExtensions;

namespace Zafiro.DivineBytes;

/// <summary>
/// Extension methods for working with the new container types
/// </summary>
public static class ContainerExtensions
{
    /// <summary>
    /// Convert a dictionary to a root container (no artificial name needed)
    /// </summary>
    public static Result<RootContainer> ToRootContainer(this Dictionary<string, IByteSource> files)
    {
        return files.ToDirectoryTree()
            .Map(container => new RootContainer(container.Resources, container.Subcontainers));
    }
    
    /// <summary>
    /// Create a named container from a root container
    /// </summary>
    public static NamedContainer WithName(this RootContainer root, string name)
    {
        return new NamedContainer(name, root.Resources, root.Subcontainers);
    }
    
    /// <summary>
    /// Create a named container from an existing container
    /// </summary>
    public static NamedContainer WithName(this INamedContainer container, string name)
    {
        return new NamedContainer(name, container.Resources, container.Subcontainers);
    }
    
    /// <summary>
    /// Convert any container to a root container
    /// </summary>
    public static RootContainer AsRoot(this INamedContainer container)
    {
        return new RootContainer(container.Resources, container.Subcontainers);
    }
    
    /// <summary>
    /// Convert root container to IContainer for compatibility with existing code
    /// </summary>
    public static INamedContainer AsContainer(this RootContainer root)
    {
        return new Container("", root.Resources, root.Subcontainers);
    }
    
    /// <summary>
    /// Get all contents as INamed for compatibility
    /// </summary>
    public static IEnumerable<INamed> GetAllContents(this INamedContainer container)
    {
        return container.Resources.Cast<INamed>().Concat(container.Subcontainers.Cast<INamed>());
    }
    
    /// <summary>
    /// Get all contents as INamed for root container
    /// </summary>
    public static IEnumerable<INamed> GetAllContents(this RootContainer container)
    {
        return container.Resources.Cast<INamed>().Concat(container.Subcontainers.Cast<INamed>());
    }
}
