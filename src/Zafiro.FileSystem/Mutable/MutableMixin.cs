using CSharpFunctionalExtensions;
using DynamicData;
using Zafiro.DataModel;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Readonly;
using File = Zafiro.FileSystem.Readonly.File;

namespace Zafiro.FileSystem.Mutable;

public static class MutableMixin
{
    public static IObservable<IChangeSet<IMutableFile, string>> Files(this IObservable<IChangeSet<IMutableNode, string>> children)
    {
        return children.Filter(x => x is IMutableFile).Cast(node => (IMutableFile)node);
    }

    public static IObservable<IChangeSet<IMutableDirectory, string>> Directories(this IObservable<IChangeSet<IMutableNode, string>> children)
    {
        return children.Filter(x => x is IMutableDirectory).Cast(node => (IMutableDirectory)node);
    }

    public static Task<Result<IEnumerable<IMutableFile>>> Files(this IMutableDirectory directory)
    {
        return directory.GetChildren().Map(nodes => nodes.OfType<IMutableFile>());
    }

    public static Task<Result<IEnumerable<IMutableDirectory>>> Directories(this IMutableDirectory directory)
    {
        return directory.GetChildren().Map(nodes => nodes.OfType<IMutableDirectory>());
    }

    public static Task<Result<IFile>> AsReadOnly(this IMutableFile file)
    {
        return file.GetContents().Map(data => (IFile)new File(file.Name, data));
    }

    public static Task<Result> CreateFileWithContents(this IMutableDirectory directory, string name, IData data)
    {
        return directory.CreateFile(name)
            .Bind(f => f.SetContents(data));
    }
    
    public static Task<Result> CreateFileWithContents(this IMutableDirectory directory, IFile file)
    {
        return directory.CreateFile(file.Name)
            .Bind(f => f.SetContents(file));
    }

    public static Task<Result<IMutableFile>> GetFile(this IMutableDirectory directory, ZafiroPath path)
    {
        return directory.Files()
            .Bind(files => files.TryFirst(file => file.Name == path.Name())
                .ToResult($"Can't find the file {path.Name()}"));
    }

    public static Task<Result<IMutableFile>> GetFile(this IMutableFileSystem fileSystem, ZafiroPath path)
    {
        return path.Parent()
            .ToResult($"Cannot get the directory of path '{path}")
            .Map(fileSystem.GetDirectory)
            .Bind(dir => dir.GetFile(path.Name()));
    }

    public static string GetKey(this IMutableNode node)
    {
        return node switch
        {
            IMutableDirectory mutableDirectory => MutableMisc.GetDirKey(mutableDirectory.Name),
            IMutableFile mutableFile => MutableMisc.GetFileKey(mutableFile.Name),
            _ => throw new ArgumentOutOfRangeException(nameof(node))
        };
    }
}

public static class MutableMisc
{
    public static string GetFileKey(string name)
    {
        return name;
    }

    public static string GetDirKey(string name)
    {
        return name + ZafiroPath.ChunkSeparator;
    }
}