using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public interface ISeaweedFS
{
    Task<Result<RootDirectory>> GetContents(string directoryPath, CancellationToken cancellationToken = default);
    Task<Result> Upload(string path, Stream stream, CancellationToken cancellationToken = default);
    Task<Result> CreateFolder(string directoryPath, CancellationToken cancellationToken = default);
    Task<Result<Stream>> GetFileContents(string filePath, CancellationToken cancellationToken = default);
    Task<Result> DeleteFile(string filePath, CancellationToken cancellationToken = default);
    Task<Result> DeleteDirectory(string directoryPath, CancellationToken cancellationToken = default);
    Task<Result<FileMetadata>> GetFileMetadata(string path, CancellationToken cancellationToken = default);
    Task<Result<bool>> PathExists(string path);
}