using Zafiro.FileSystem.Mutable;
using DirectoryBase = Zafiro.FileSystem.Mutable.DirectoryBase;

namespace Zafiro.FileSystem.Local;

public class Directory : DirectoryBase
{
    public Directory(IDirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;
    }

    public IDirectoryInfo DirectoryInfo { get; }

    public override string Name => DirectoryInfo.Name.Replace("\\", "");

    public override bool IsHidden
    {
        get
        {
            if (DirectoryInfo.Parent == null)
            {
                return false;
            }

            return (DirectoryInfo.Attributes & FileAttributes.Hidden) != 0;
        }
    }

    public override async Task<Result<bool>> HasSubdirectory(string name)
    {
        return Result.Try(() =>
        {
            var fs = DirectoryInfo.FileSystem;
            return DirectoryInfo.FileSystem.Directory.Exists(fs.Path.Combine(DirectoryInfo.FullName, name));
        });
    }

    protected override async Task<Result<IMutableDirectory>> CreateSubdirectoryCore(string name)
    {
        return Result.Try(() => DirectoryInfo.CreateSubdirectory(name))
            .Map(directoryInfo => (IMutableDirectory)new Directory(directoryInfo));
    }

    protected override async Task<Result> DeleteFileCore(string name)
    {
        return Result.Try(() =>
        {
            var filePath = DirectoryInfo.FileSystem.Path.Combine(DirectoryInfo.FullName, name);
            var file = DirectoryInfo.FileSystem.FileInfo.New(filePath);
            file.Delete();
        });
    }

    protected override async Task<Result> DeleteSubdirectoryCore(string name)
    {
        return Result.Try(() =>
        {
            var dirPath = DirectoryInfo.FileSystem.Path.Combine(DirectoryInfo.FullName, name);
            var dir = DirectoryInfo.FileSystem.DirectoryInfo.New(dirPath);
            dir.Delete();
        });
    }

    public override async Task<Result<IEnumerable<IMutableNode>>> GetChildren(CancellationToken cancellationToken = default)
    {
        return Result.Try(() =>
        {
            var files = DirectoryInfo.GetFiles().Select(info => (IMutableNode)new File(info)).Where(x => !x.IsHidden);
            var dirs = DirectoryInfo.GetDirectories().Select(x => (IMutableNode)new Directory(x));
            var nodes = files.Concat(dirs);
            return nodes;
        });
    }

    public override async Task<Result<bool>> HasFile(string name)
    {
        return Result.Try(() =>
        {
            var fs = DirectoryInfo.FileSystem;
            return DirectoryInfo.FileSystem.File.Exists(fs.Path.Combine(DirectoryInfo.FullName, name));
        });
    }

    protected override async Task<Result<IMutableFile>> CreateFileCore(string name)
    {
        return Result.Try(() =>
        {
            var fs = DirectoryInfo.FileSystem;
            var file = new File(fs.FileInfo.New(fs.Path.Combine(DirectoryInfo.FullName, name)));
            return (IMutableFile)file;
        });
    }

    public override string? ToString()
    {
        return DirectoryInfo.ToString();
    }
}