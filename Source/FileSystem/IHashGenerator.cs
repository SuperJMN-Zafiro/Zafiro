namespace FileSystem;

public interface IHashGenerator
{
    byte[] ComputeHash(Stream stream);
}