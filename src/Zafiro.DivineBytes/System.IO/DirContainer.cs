using System.IO.Abstractions;

namespace Zafiro.DivineBytes.System.IO;

public class DirContainer(IDirectoryInfo directoryInfo) : INamedContainer
{
    public string Name => directoryInfo.Name;
    
    // Implement new IContainer interface
    public IEnumerable<INamedContainer> Subcontainers => directoryInfo
        .GetDirectories()
        .Select(info => new DirContainer(info));
        
    public IEnumerable<INamedByteSource> Resources => directoryInfo
        .GetFiles()
        .Select(info => new FileContainer(info));
}