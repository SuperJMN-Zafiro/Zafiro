namespace Zafiro.DivineBytes;

public static class DirectoryExtensions
{
    public static IEnumerable<INamedByteSource> Files(this IDirectory directory)
        => directory.Children.OfType<INamedByteSource>();

    public static IEnumerable<IDirectory> Directories(this IDirectory directory)
        => directory.Children.OfType<IDirectory>();

    public static IEnumerable<INamedByteSource> FilesRecursive(this IDirectory directory)
        => directory.Files().Concat(directory.Directories().SelectMany(d => d.FilesRecursive()));

    public static IEnumerable<INamedWithPath> ChildrenWithPathsRecursive(this IDirectory directory)
        => directory.ChildrenRelativeTo(Path.Empty);

    public static IEnumerable<INamedByteSourceWithPath> FilesWithPathsRecursive(this IDirectory directory)
        => directory.ChildrenRelativeTo(Path.Empty).OfType<INamedByteSourceWithPath>();

    public static IEnumerable<INamedWithPath> ChildrenRelativeTo(this IDirectory directory, Path path)
    {
        var myFiles = directory.Files().Select(file => new NamedByteSourceWithPath(path, file));
        var filesInSubDirs = directory.Directories()
            .SelectMany(d => d.ChildrenRelativeTo(path.Combine(d.Name)));

        return myFiles.Concat(filesInSubDirs);
    }
}