namespace Zafiro.DivineBytes.Unix;

public class DefaultMetadataResolver : IMetadataResolver
{
    public Metadata ResolveDirectory(INamedContainer dir)
    {
        return new Metadata(Permission.All, 0);
    }

    public Metadata ResolveFile(INamedByteSource file)
    {
        return new Metadata(Permission.All, 0);
    }
}