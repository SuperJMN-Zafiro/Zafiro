using System.Reactive.Linq;
using CSharpFunctionalExtensions;

namespace Zafiro.DivineBytes.Permissioned;
// Capacidad que puede añadirse a cualquier elemento
public interface IPermissioned
{
    UnixPermissions Permissions { get; }
}

// Objeto valor para permisos estilo Unix
public struct UnixPermissions
{
    public bool OwnerRead   { get; }
    public bool OwnerWrite  { get; }
    public bool OwnerExec   { get; }
    public bool GroupRead   { get; }
    public bool GroupWrite  { get; }
    public bool GroupExec   { get; }
    public bool OtherRead   { get; }
    public bool OtherWrite  { get; }
    public bool OtherExec   { get; }

    public UnixPermissions(bool ownerRead, bool ownerWrite, bool ownerExec,
        bool groupRead, bool groupWrite, bool groupExec,
        bool otherRead, bool otherWrite, bool otherExec)
    {
        OwnerRead  = ownerRead;
        OwnerWrite = ownerWrite;
        OwnerExec  = ownerExec;
        GroupRead  = groupRead;
        GroupWrite = groupWrite;
        GroupExec  = groupExec;
        OtherRead  = otherRead;
        OtherWrite = otherWrite;
        OtherExec  = otherExec;
    }
}

// 1. Nuevas clases inmóviles: sólo datos
public class UnixDirectory : IDirectory, IPermissioned, IOwned
{
    public string Name { get; }
    public UnixPermissions Permissions { get; }

    public IEnumerable<UnixDirectory> Subdirectories { get; }
    public IEnumerable<UnixFile> Files { get; }

    public IEnumerable<INamed> Children 
        => Subdirectories.Cast<INamed>().Concat(Files);

    internal UnixDirectory(
        string name,
        int ownerId,
        UnixPermissions permissions,
        IEnumerable<UnixDirectory> subdirs,
        IEnumerable<UnixFile> files)
    {
        Name           = name;
        Permissions    = permissions;
        Subdirectories = subdirs;
        Files          = files;
    }

    public int OwnerId { get; }
}

public interface IOwned
{
    public int OwnerId { get; }
}

public class UnixFile : INamedByteSource, IPermissioned, IOwned
{
    private readonly INamedByteSource inner;
    public string Name => inner.Name;
    public UnixPermissions Permissions { get; }

    public UnixFile(INamedByteSource inner, UnixPermissions perms, int ownerId)
    {
        this.inner      = inner;
        Permissions = perms;
        OwnerId = ownerId;
    }

    public IObservable<byte[]> Bytes
        => Permissions.OwnerRead 
           ? inner.Bytes 
           : Observable.Empty<byte[]>();

    public IDisposable Subscribe(IObserver<byte[]> observer)
    {
        return inner.Subscribe(observer);
    }

    public int OwnerId { get; }
}

public record Metadata(UnixPermissions Permissions, int OwnerId);

public interface IMetadataResolver
{
    Metadata ResolveDirectory(IDirectory dir);
    Metadata ResolveFile(INamedByteSource file);
}

public class UnixTreeBuilder
{
    private readonly IMetadataResolver resolver;

    public UnixTreeBuilder(IMetadataResolver resolver)
        => this.resolver = resolver;

    public UnixDirectory Build(IDirectory root)
        => BuildDirectory(root);

    private UnixDirectory BuildDirectory(IDirectory dir)
    {
        var md = resolver.ResolveDirectory(dir);

        var subdirs = dir.Subdirectories
            .Select(BuildDirectory);

        var files = dir.Files
            .Select(f =>
            {
                var fm = resolver.ResolveFile(f);
                return new UnixFile(f, fm.Permissions, fm.OwnerId);
            });

        return new UnixDirectory(dir.Name, md.OwnerId, md.Permissions, subdirs, files);
    }
}
