namespace Zafiro.DivineBytes.Unix;

public class UnixDirectory : IDirectory, IPermissioned, IOwned
{
    public string Name { get; }
    public UnixPermissions Permissions { get; }

    public IEnumerable<UnixDirectory> Subdirectories { get; }
    public IEnumerable<UnixFile> Files { get; }

    public IEnumerable<INamed> Children 
        => Subdirectories.Cast<INamed>().Concat(Files);

    public UnixDirectory(
        string name,
        int ownerId,
        UnixPermissions permissions,
        IEnumerable<UnixDirectory> subdirs,
        IEnumerable<UnixFile> files)
    {
        Name           = name;
        OwnerId = ownerId;
        Permissions    = permissions;
        Subdirectories = subdirs;
        Files          = files;
    }

    public int OwnerId { get; }
}