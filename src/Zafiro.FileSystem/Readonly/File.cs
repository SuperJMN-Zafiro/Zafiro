using Zafiro.DataModel;

namespace Zafiro.FileSystem.Readonly;

public class File : IFile
{
    public File(string name, IData data)
    {
        Name = name;
        Data = data;
    }

    public IData Data { get; }
    public string Name { get; }
    public IObservable<byte[]> Bytes => Data.Bytes;
    public long Length => Data.Length;

    public override string ToString()
    {
        return Name;
    }
}