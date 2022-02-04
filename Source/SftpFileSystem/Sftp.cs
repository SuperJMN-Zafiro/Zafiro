using System.IO.Abstractions;
using System.Net;
using System.Xml;
using CSharpFunctionalExtensions;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using FileSystem;

namespace SftpFileSystem;

public class Sftp
{
    private const string HashesFilename = "hashes.dat";

    private static readonly IExtendedXmlSerializer ExtendedXmlSerializer = new ConfigurationContainer()
        .UseOptimizedNamespaces()
        .Create();

    private readonly Credentials credentials;
    private readonly DnsEndPoint endpoint;

    public Sftp(DnsEndPoint endpoint, Credentials credentials)
    {
        this.endpoint = endpoint;
        this.credentials = credentials;
    }

    public async Task CopyDirectory(string origin, string destination)
    {
        using var destFs = FileSystem.Connect(endpoint.Host, endpoint.Port, credentials);
        var fileSystemPathTranslator = new FileSystemPathTranslator();
        var fileSystemComparer = new FileSystemComparer();
        var smartFileManager = new SmartFileManager(endpoint.Host);
        var sourceFs = new System.IO.Abstractions.FileSystem();
        var hashesFile = sourceFs.FileInfo.FromFileName(HashesFilename);
        var destinationDir = destFs.DirectoryInfo.FromDirectoryName(destination);
        var originDir = sourceFs.DirectoryInfo.FromDirectoryName(origin);
        var bulkCopier = new BulkCopier(fileSystemComparer, fileSystemPathTranslator,
            context => smartFileManager);
        await LoadHashes(smartFileManager, hashesFile);
        var result = await bulkCopier.Copy(originDir, destinationDir);
        await result.OnSuccessTry(() => SaveHashes(hashesFile, smartFileManager.Hashes));
    }

    private static Task SaveHashes(IFileInfo file, Dictionary<SmartFileManager.Key, byte[]> hashes)
    {
        using var stream = file.Create();
        using var xmlWriter = XmlWriter.Create(stream);
        ExtendedXmlSerializer.Serialize(xmlWriter, hashes);
        return Task.CompletedTask;
    }

    private static Task LoadHashes(SmartFileManager smartFileManager, IFileInfo file)
    {
        Result.Try(() =>
            {
                if (!file.Exists)
                {
                    return new Dictionary<SmartFileManager.Key, byte[]>();
                }

                using var stream = file.OpenRead();
                return ExtendedXmlSerializer.Deserialize<Dictionary<SmartFileManager.Key, byte[]>>(stream);
            })
            .OnSuccessTry(hashes => smartFileManager.Hashes = hashes);

        return Task.CompletedTask;
    }
}