using System.IO.Abstractions;
using System.Net;
using CSharpFunctionalExtensions;
using FileSystem;

namespace SftpFileSystem;

public class Sftp
{
    private const string HashesFilename = "hashes.dat";

    private static readonly System.IO.Abstractions.FileSystem SourceFs = new();
    private static readonly IFileInfo HashesFile = SourceFs.FileInfo.FromFileName(HashesFilename);

    private readonly Credentials credentials;
    private readonly DnsEndPoint endpoint;

    public Sftp(DnsEndPoint endpoint, Credentials credentials)
    {
        this.endpoint = endpoint;
        this.credentials = credentials;
    }

    public async Task<Result> CopyDirectory(string origin, string destination)
    {
        var result = await FileSystem.Connect(endpoint.Host, endpoint.Port, credentials)
            .Bind(fs =>
            {
                using (fs)
                {
                    var fileSystemPathTranslator = new FileSystemPathTranslator();
                    var fileSystemComparer = new FileSystemComparer();
                    var destinationDir = fs.DirectoryInfo.FromDirectoryName(destination);
                    var originDir = SourceFs.DirectoryInfo.FromDirectoryName(origin);
                    var smartFileManager = StorageWrapper.Create(HashesFile,
                        hashes => new SmartFileManager(endpoint.Host, new FileManager(), hashes));
                    var bulkCopier = new BulkCopier(fileSystemComparer, fileSystemPathTranslator, smartFileManager);

                    return Result.Success()
                        .Bind(() => bulkCopier.Copy(originDir, destinationDir));
                }
            });

        return result;
    }
}