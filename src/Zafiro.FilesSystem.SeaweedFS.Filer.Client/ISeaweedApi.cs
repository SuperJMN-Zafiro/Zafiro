using Refit;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

[Headers("Accept: application/json")]
public interface ISeaweedApi
{
    [Get("/{directoryPath}?pretty=y")]
    Task<RootDirectory> GetContents(string directoryPath, CancellationToken cancellationToken);

    [Get("/{filePath}?metadata=true&pretty=y")]
    Task<FileMetadata> GetFileMetadata(string filePath, CancellationToken cancellationToken);

    [Multipart]
    [Post("/{path}")]
    Task Upload(string path, Stream stream, CancellationToken cancellationToken);

    [Multipart]
    [Post("/{directoryPath}/")]
    Task CreateFolder(string directoryPath, CancellationToken cancellationToken);

    [Delete("/{filePath}")]
    Task DeleteFile(string filePath, CancellationToken cancellationToken);

    [Delete("/{directoryPath}?recursive=true")]
    Task DeleteFolder(string directoryPath, CancellationToken cancellationToken);
}