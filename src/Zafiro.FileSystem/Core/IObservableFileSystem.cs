namespace Zafiro.FileSystem.Core;

public interface IObservableFileSystem : IZafiroFileSystem
{
    IObservable<FileSystemChange> Changed { get; }
}