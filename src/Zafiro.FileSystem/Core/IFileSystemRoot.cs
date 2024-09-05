using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.Core;

public interface IFileSystemRoot : IObservableFileSystem
{
    IZafiroFile GetFile(ZafiroPath path);
    IZafiroDirectory GetDirectory(ZafiroPath path);
    Task<Result<IEnumerable<IZafiroFile>>> GetFiles(ZafiroPath path, CancellationToken ct = default);
    Task<Result<IEnumerable<IZafiroDirectory>>> GetDirectories(ZafiroPath path, CancellationToken ct = default);
}