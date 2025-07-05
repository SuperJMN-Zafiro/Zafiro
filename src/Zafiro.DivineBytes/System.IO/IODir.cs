using System.IO.Abstractions;

namespace Zafiro.DivineBytes.System.IO;

public class IoDir(IDirectoryInfo directoryInfo) : IContainer
{
    public string Name => directoryInfo.Name;
    public IEnumerable<INamed> Children => directoryInfo
        .GetFiles()
        .Select(info => new IoFile(info))
        .Concat<INamed>(directoryInfo
            .GetDirectories()
            .Select(info => new IoDir(info)));
}