using System.Security.Cryptography;

namespace FileSystem;

public class HashGenerator : IHashGenerator
{
    private static readonly SHA1 HashProvider = SHA1.Create();
    
    public Task<byte[]> ComputeHash(Stream stream)
    {
        return HashProvider.ComputeHashAsync(stream);
    }

    public async Task<byte[]> ComputeHash(Func<Stream> streamFactory)
    {
        await using var stream = streamFactory();
        return await ComputeHash(stream);
    }
}