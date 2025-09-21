namespace Zafiro.DivineBytes.Unix;

public class UnixDirectory : INamedContainer, IPermissioned, IOwned
{
    public string Name { get; }
    public UnixPermissions Permissions { get; }

    public IEnumerable<UnixDirectory> Subdirectories { get; }
    public IEnumerable<UnixFile> Files { get; }

    // Implement new IContainer interface
    public IEnumerable<INamedContainer> Subcontainers => Subdirectories;
    public IEnumerable<INamedByteSource> Resources => Files;

    // Keep Children for backward compatibility if needed
    public IEnumerable<INamed> Children
        => Subdirectories.Cast<INamed>().Concat(Files);

    public UnixDirectory(
        string name,
        int ownerId,
        UnixPermissions permissions,
        IEnumerable<UnixDirectory> subdirs,
        IEnumerable<UnixFile> files)
    {
        Name = name;
        OwnerId = ownerId;
        Permissions = permissions;
        Subdirectories = subdirs.ToList();
        Files = files.ToList();
    }

    public int OwnerId { get; }
}