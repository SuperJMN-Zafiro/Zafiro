using System.IO.Abstractions;
using System.Xml;
using CSharpFunctionalExtensions;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;

namespace FileSystem;

public sealed class FileBasedCachingFileManager : IFileManager, IDisposable
{
    private static readonly IExtendedXmlSerializer ExtendedXmlSerializer = new ConfigurationContainer()
        .UseOptimizedNamespaces()
        .Create();

    private readonly ICachingFileManager cachedFileManager;
    private readonly IFileInfo hashesFile;

    private FileBasedCachingFileManager(ICachingFileManager cachingFileManager, IFileInfo hashesFile)
    {
        cachedFileManager = cachingFileManager;
        this.hashesFile = hashesFile;
    }

    public void Dispose()
    {
        Result.Try(SaveHashes);
    }

    public Task Copy(IFileInfo source, IFileInfo destination)
    {
        return cachedFileManager.Copy(source, destination);
    }

    public void Delete(IFileInfo file)
    {
        cachedFileManager.Delete(file);
    }

    public static FileBasedCachingFileManager Create(IFileInfo hashesFile,
        Func<Dictionary<CacheKey, byte[]>, ICachingFileManager> factory)
    {
        var emptyHashes = new Dictionary<CacheKey, byte[]>();

        var storageWrapper = Result
            .Try(() => LoadHashes(hashesFile))
            .Match(r => new FileBasedCachingFileManager(factory(r), hashesFile),
                s => new FileBasedCachingFileManager(factory(emptyHashes), hashesFile));

        return storageWrapper;
    }

    private static Dictionary<CacheKey, byte[]> LoadHashes(IFileInfo hashesFile)
    {
        if (!hashesFile.Exists)
        {
            return new Dictionary<CacheKey, byte[]>();
        }

        using var stream = hashesFile.OpenRead();
        return ExtendedXmlSerializer.Deserialize<Dictionary<CacheKey, byte[]>>(stream);
    }

    private void SaveHashes()
    {
        using var stream = hashesFile.Create();
        using var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings {Indent = true});
        ExtendedXmlSerializer.Serialize(xmlWriter, cachedFileManager.Cache);
    }
}