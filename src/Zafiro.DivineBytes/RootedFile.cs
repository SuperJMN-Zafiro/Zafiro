namespace Zafiro.DivineBytes;

public class RootedFile(ZafiroPath path, IFile file) : IFile, IRooted
{
    public IObservable<byte[]> Bytes => file.Bytes;
    public IFile Value => file;
    public ZafiroPath Path => path;
    public string Name => file.Name;

    public override string ToString()
    {
        return this.FullPath();
    }
}