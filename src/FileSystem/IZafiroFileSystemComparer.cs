namespace Zafiro.FileSystem;

public interface IZafiroFileSystemComparer
{
    Task<IEnumerable<ZafiroFileDiff>> Diff(IZafiroDirectory origin, IZafiroDirectory destination);
}