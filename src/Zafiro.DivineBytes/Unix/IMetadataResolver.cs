namespace Zafiro.DivineBytes.Unix;

public interface IMetadataResolver
{
    Metadata ResolveDirectory(IContainer dir);
    Metadata ResolveFile(INamedByteSource file);
}