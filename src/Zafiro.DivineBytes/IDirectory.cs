namespace Zafiro.DivineBytes;

public interface IDirectory : INamed
{
    public IEnumerable<INamedByteSource> Children { get; }
}