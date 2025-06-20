using Zafiro.DivineBytes;
using Zafiro.FileSystem.Readonly;

namespace Zafiro.Deployment.New.Platforms.Linux.Adapters;

internal class FileAdapter(string fileName, IByteSource byteSource) : IFile
{
    public IObservable<byte[]> Bytes => byteSource.Bytes;
    public long Length => byteSource.GetLength().Result.GetValueOrDefault(-1);
    public string Name => fileName;
}