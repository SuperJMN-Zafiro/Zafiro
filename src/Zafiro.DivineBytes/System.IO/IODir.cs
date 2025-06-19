using System.IO.Abstractions;

namespace Zafiro.DivineBytes.System.IO;

public class IODir(IDirectoryInfo directoryInfo) : IDirectory
{
    public string Name => directoryInfo.Name;
    public IEnumerable<INamed> Children => directoryInfo
        .GetFiles()
        .Select(info => new IOFile(info))
        .Concat<INamed>(directoryInfo
            .GetDirectories()
            .Select(info => new IODir(info)));
}