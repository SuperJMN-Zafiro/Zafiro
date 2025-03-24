using System.IO.Abstractions;

namespace Zafiro.DivineBytes.Local;

public class DotnetDir(IDirectoryInfo directory) : IDirectory
{
    public string Name => directory.Name;
    public IEnumerable<INode> Children => directory.GetFiles()
        .Select(info => (INode)new DotnetFile(info)).Concat(directory.GetDirectories().Select(x => new DotnetDir(x)));
}