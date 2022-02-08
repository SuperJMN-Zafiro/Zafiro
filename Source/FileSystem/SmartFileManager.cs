namespace FileSystem;

public interface ICachingFileManager : IFileManager
{
    Dictionary<CacheKey, byte[]> Cache { get; }
}