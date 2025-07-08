namespace Zafiro.DivineBytes.Unix;

public interface IMetadataResolver
{
    Metadata ResolveDirectory(INamedContainer dir);
    Metadata ResolveFile(INamedByteSource file);
}