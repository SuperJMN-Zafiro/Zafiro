namespace Zafiro.DivineBytes.Unix;

public static class Extensions
{
    public static UnixDirectory ToUnixDirectory(
        this IDirectory dir,
        IMetadataResolver resolver)
    {
        var builder = new UnixTreeBuilder(resolver);
        return builder.Build(dir);
    }
}