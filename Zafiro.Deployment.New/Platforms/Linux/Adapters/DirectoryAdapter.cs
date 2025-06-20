using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Readonly;

namespace Zafiro.Deployment.New.Platforms.Linux.Adapters;

internal class DirectoryAdapter : IDirectory
{
    private readonly DivineBytes.IDirectory divineDir;

    public DirectoryAdapter(DivineBytes.IDirectory divineDir)
    {
        this.divineDir = divineDir;
    }

    public string Name => divineDir.Name;
    public IEnumerable<INode> Children => divineDir.Children.Select(node =>
    {
        return node switch
        {
            DivineBytes.IDirectory directory => (INode) new DirectoryAdapter(directory),
            DivineBytes.INamedByteSource file => new FileAdapter(file.Name, file),
            _ => throw new ArgumentOutOfRangeException(nameof(node))
        };
    });
}