namespace Zafiro.DivineBytes.Unix;

public static class Extensions
{
    public static UnixDirectory ToUnixDirectory(
        this INamedContainer dir,
        IMetadataResolver? resolver = null)
    {
        var builder = new UnixTreeBuilder(resolver ?? new DefaultMetadataResolver());
        return builder.Build(dir);
    }

    public static UnixDirectory ToUnixDirectory(
        this IContainer container,
        IMetadataResolver? resolver = null)
    {
        var named = new Container("", container.Resources, container.Subcontainers);
        return named.ToUnixDirectory(resolver);
    }
}
