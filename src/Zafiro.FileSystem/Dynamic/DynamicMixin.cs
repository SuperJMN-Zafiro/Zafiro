using DynamicData;
using Zafiro.FileSystem.Readonly;

namespace Zafiro.FileSystem.Dynamic;

public static class DynamicMixin
{
    public static IObservable<IChangeSet<IFile, string>> AllFiles(this IDynamicDirectory directory)
    {
        return directory.Files.MergeChangeSets(directory.AllDirectories().MergeManyChangeSets(x => x.AllFiles()));
    }

    public static IObservable<IChangeSet<IDynamicDirectory, string>> AllDirectories(this IDynamicDirectory directory)
    {
        return directory.Directories.MergeChangeSets(directory.Directories.MergeManyChangeSets(x => x.AllDirectories()));
    }
}