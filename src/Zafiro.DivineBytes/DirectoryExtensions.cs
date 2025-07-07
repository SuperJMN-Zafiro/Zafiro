namespace Zafiro.DivineBytes;

public static class DirectoryExtensions
{
    public static IEnumerable<INamedByteSource> FilesRecursive(this IContainer container)
        => container.Resources.Concat(container.Subcontainers.SelectMany(d => d.FilesRecursive()));

    public static IEnumerable<INamedWithPath> ChildrenWithPathsRecursive(this IContainer container)
        => container.ChildrenRelativeTo(Path.Empty);

    public static IEnumerable<INamedByteSourceWithPath> FilesWithPathsRecursive(this IContainer container)
        => container.ChildrenRelativeTo(Path.Empty).OfType<INamedByteSourceWithPath>();

    public static IEnumerable<INamedWithPath> ChildrenRelativeTo(this IContainer container, Path path)
    {
        // Include files as INamedByteSourceWithPath (which implements INamedWithPath)
        var myFiles = container.Resources.Select(file => new NamedByteSourceWithPath(path, file));
        
        // Include subdirectories as INamedWithPath and recursively their children
        var subcontainerResults = container.Subcontainers.SelectMany(subcontainer =>
        {
            var subcontainerPath = path.Combine(subcontainer.Name);
            // Create a simple container wrapper for the subdirectory itself
            var containerAsPath = new ContainerWithPath(subcontainerPath, subcontainer.Name);
            // Return the subdirectory itself plus its children
            return new INamedWithPath[] { containerAsPath }
                .Concat(subcontainer.ChildrenRelativeTo(subcontainerPath));
        });

        return myFiles.Concat(subcontainerResults);
    }
}

// Helper class to represent a container as INamedWithPath
internal record ContainerWithPath(Path Path, string Name) : INamedWithPath;
