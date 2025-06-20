namespace Zafiro.DivineBytes;

public interface IDirectory : INamed
{
    public IEnumerable<INamed> Children { get; }
    IEnumerable<IDirectory> Subdirectories => Children.OfType<IDirectory>();
    IEnumerable<INamedByteSource> Files => Children.OfType<INamedByteSource>();
}