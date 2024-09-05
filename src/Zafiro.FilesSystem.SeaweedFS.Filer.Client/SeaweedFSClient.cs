using System.Net;
using System.Runtime.Caching;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Refit;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public class SeaweedFSClient : ISeaweedFS
{
    private readonly MemoryCache fileMetadatas = new("metadatas");
    private readonly HttpClient httpClient;
    private readonly ISeaweedApi inner;

    public SeaweedFSClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        inner = RestService.For<ISeaweedApi>(httpClient, new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
            {
                Converters =
                {
                    new FileSystemEntryConverter()
                }
            })
        });
    }

    public Task<Result<RootDirectory>> GetContents(string directoryPath, CancellationToken cancellationToken = default)
    {
        return Result.Try(async () =>
        {
            var contents = await inner.GetContents(directoryPath, cancellationToken);
            var files = contents.Entries?.OfType<FileMetadata>() ?? [];

            foreach (var file in files)
            {
                var policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
                };
                fileMetadatas.Add(file.FullPath.StartsWith("/") ? file.FullPath[1..] : file.FullPath, file, policy);
            }

            return contents;
        });
    }

    public Task<Result> Upload(string path, Stream stream, CancellationToken cancellationToken = default)
    {
        return Result.Try(() => inner.Upload(path, stream, cancellationToken));
    }

    public Task<Result> CreateFolder(string directoryPath, CancellationToken cancellationToken = default)
    {
        return Result.Try(() => inner.CreateFolder(directoryPath, cancellationToken));
    }

    public Task<Result> DeleteDirectory(string directoryPath, CancellationToken cancellationToken = default)
    {
        return Result.Try(() => inner.DeleteFolder(directoryPath, cancellationToken));
    }

    public Task<Result<Stream>> GetFileContents(string filePath, CancellationToken cancellationToken = default)
    {
        return Result.Try(() => httpClient.GetStreamAsync(filePath, cancellationToken));
    }

    public Task<Result> DeleteFile(string filePath, CancellationToken cancellationToken = default)
    {
        fileMetadatas.Remove(filePath);

        return Result.Try(() => inner.DeleteFile(filePath, cancellationToken));
    }

    public Task<Result<FileMetadata>> GetFileMetadata(string path, CancellationToken cancellationToken = default)
    {
        if (fileMetadatas.Get(path) is FileMetadata metadata)
        {
            return Task.FromResult(Result.Success(metadata));
        }

        return Result.Try(() => inner.GetFileMetadata(path, cancellationToken));
    }

    public async Task<Result<bool>> PathExists(string path)
    {
        try
        {
            return await GetFileMetadata(path).Map(_ => true);
        }
        catch (ApiException e)
        {
            if (e.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        return true;
    }
}