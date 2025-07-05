using System.IO.Abstractions;

namespace Zafiro.DivineBytes.System.IO;

public class IoDir(IDirectoryInfo directoryInfo) : IContainer
{
    public string Name => directoryInfo.Name;
    
    // Implement new IContainer interface
    public IEnumerable<IContainer> Subcontainers => directoryInfo
        .GetDirectories()
        .Select(info => new IoDir(info));
        
    public IEnumerable<INamedByteSource> Resources => directoryInfo
        .GetFiles()
        .Select(info => new IoFile(info));
}