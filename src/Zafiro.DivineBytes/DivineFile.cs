namespace Zafiro.DivineBytes;

public class DivineFile : IFile
{
    public string Name { get; }
    private readonly IData data;

    public DivineFile(string name, IData data)
    {
        Name = name;
        this.data = data;
    }

    public IObservable<byte[]> Bytes => data.Bytes;
}