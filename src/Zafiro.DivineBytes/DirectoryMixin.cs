namespace Zafiro.DivineBytes;

public static class DirectoryMixin
{
    public static IEnumerable<INamedByteSource> Files(this IDirectory directory)
    {
        return directory.Children.OfType<INamedByteSource>();
    }

    public static IEnumerable<NamedByteSourceWithPath> RootedFiles(this IDirectory directory)
    {
        return directory.RootedFilesRelativeTo(Path.Empty);
    }
    
    public static IEnumerable<INamedByteSource> AllFiles(this IDirectory directory)
    {
        return directory.Files().Concat(directory.Directories().SelectMany(d => d.AllFiles()));
    }

    public static IEnumerable<NamedByteSourceWithPath> RootedFilesRelativeTo(this IDirectory directory, Path path)
    {
        var myFiles = directory.Children.OfType<INamedByteSource>().Select(file => new NamedByteSourceWithPath(path, file));
        var filesInSubDirs = directory.Directories().SelectMany(d => d.RootedFilesRelativeTo(path.Combine(d.Name)));

        return myFiles.Concat(filesInSubDirs);
    }

    public static IEnumerable<IDirectory> Directories(this IDirectory directory)
    {
        return directory.Children.OfType<IDirectory>();
    }
}