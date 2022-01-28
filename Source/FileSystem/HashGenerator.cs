using System.Security.Cryptography;

namespace FileSystem;

public class HashGenerator : IHashGenerator
{
    private static readonly SHA1 HashProvider = SHA1.Create();
    
    public byte[] ComputeHash(Stream stream)
    {
        return HashProvider.ComputeHash(stream);
    }
}