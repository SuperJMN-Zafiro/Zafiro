namespace Zafiro.DivineBytes;

public interface IContainer
{
    IEnumerable<INamedContainer> Subcontainers { get; }
    IEnumerable<INamedByteSource> Resources { get; }
}