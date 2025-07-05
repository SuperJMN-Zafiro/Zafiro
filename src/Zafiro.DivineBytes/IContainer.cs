namespace Zafiro.DivineBytes;

public interface IContainer : INamed
{
    IEnumerable<IContainer> Subcontainers { get; }
    IEnumerable<INamedByteSource> Resources { get; }
}