namespace Zafiro.DivineBytes;

public interface IDirectory : INamed
{
    public IEnumerable<INamed> Children { get; }
}