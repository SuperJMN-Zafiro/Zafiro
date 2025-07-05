namespace Zafiro.DivineBytes.Unix;

public class UnixTreeBuilder
{
    private readonly IMetadataResolver resolver;

    public UnixTreeBuilder(IMetadataResolver resolver)
        => this.resolver = resolver;

    public UnixDirectory Build(IContainer root)
        => BuildDirectory(root);

    private UnixDirectory BuildDirectory(IContainer dir)
    {
        var md = resolver.ResolveDirectory(dir);

        var subdirs = dir.Subcontainers
            .Select(BuildDirectory);

        var files = dir.Resources
            .Select(f =>
            {
                var fm = resolver.ResolveFile(f);
                return new UnixFile(f, fm.Permissions, fm.OwnerId);
            });

        return new UnixDirectory(dir.Name, md.OwnerId, md.Permissions, subdirs, files);
    }
}