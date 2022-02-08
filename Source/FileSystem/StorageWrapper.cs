using System.IO.Abstractions;
using System.Xml;
using CSharpFunctionalExtensions;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;

namespace FileSystem;

public sealed class StorageWrapper : IFileManager, IDisposable
{
    private static readonly IExtendedXmlSerializer ExtendedXmlSerializer = new ConfigurationContainer()
        .UseOptimizedNamespaces()
        .Create();

    private readonly SmartFileManager cachedFileManager;
    private readonly IFileInfo hashesFile;

    private StorageWrapper(SmartFileManager smartFileManager, IFileInfo hashesFile)
    {
        cachedFileManager = smartFileManager;
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

    public static StorageWrapper Create(IFileInfo hashesFile,
        Func<Dictionary<SmartFileManager.Key, byte[]>, SmartFileManager> factory)
    {
        var emptyHashes = new Dictionary<SmartFileManager.Key, byte[]>();

        var storageWrapper = Result
            .Try(() => LoadHashes(hashesFile))
            .Match(r => new StorageWrapper(factory(r), hashesFile),
                s => new StorageWrapper(factory(emptyHashes), hashesFile));

        return storageWrapper;
    }

    private static Dictionary<SmartFileManager.Key, byte[]> LoadHashes(IFileInfo hashesFile)
    {
        if (!hashesFile.Exists)
        {
            return new Dictionary<SmartFileManager.Key, byte[]>();
        }

        using var stream = hashesFile.OpenRead();
        return ExtendedXmlSerializer.Deserialize<Dictionary<SmartFileManager.Key, byte[]>>(stream);
    }

    private void SaveHashes()
    {
        using var stream = hashesFile.Create();
        using var xmlWriter = XmlWriter.Create(stream);
        ExtendedXmlSerializer.Serialize(xmlWriter, cachedFileManager.Hashes);
    }
}