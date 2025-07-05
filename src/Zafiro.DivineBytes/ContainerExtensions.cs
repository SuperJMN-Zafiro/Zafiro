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
            .Map(container => new RootContainer(container.Children));
    }
    
    /// <summary>
    /// Create a named container from a root container
    /// </summary>
    public static NamedContainer WithName(this RootContainer root, string name)
    {
        return new NamedContainer(name, root.Children);
    }
    
    /// <summary>
    /// Create a named container from an existing container
    /// </summary>
    public static NamedContainer WithName(this IContainer container, string name)
    {
        return new NamedContainer(name, container.Children);
    }
    
    /// <summary>
    /// Convert any container to a root container
    /// </summary>
    public static RootContainer AsRoot(this IContainer container)
    {
        return new RootContainer(container.Children);
    }
    
    /// <summary>
    /// Convert root container to IContainer for compatibility with existing code
    /// </summary>
    public static IContainer AsContainer(this RootContainer root)
    {
        return new Container("", root.Children.ToArray());
    }
}
