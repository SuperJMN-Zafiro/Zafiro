namespace FileSystem;

public interface IStreamStore
{
    public Task Create(string path, Func<Stream> getContents);
    public Task Replace(string path, Func<Stream> getContents);
    public Task Delete(string path);
}