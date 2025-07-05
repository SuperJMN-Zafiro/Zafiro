namespace Zafiro.DivineBytes;

public interface IContainer : INamed
{
    public IEnumerable<INamed> Children { get; }
    IEnumerable<IContainer> Subdirectories => Children.OfType<IContainer>();
    IEnumerable<INamedByteSource> Files => Children.OfType<INamedByteSource>();
}