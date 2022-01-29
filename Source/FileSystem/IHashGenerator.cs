namespace FileSystem;

public interface IHashGenerator
{
    Task<byte[]> ComputeHash(Stream stream);
    Task<byte[]> ComputeHash(Func<Stream> streamFactory);
}