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
public class PermissionedDirectory : IDirectory, IPermissioned
{
    public string Name { get; }
    public UnixPermissions Permissions { get; }

    // Exponemos ya las dos colecciones tipadas (fase 1 revisitada)
    public IEnumerable<PermissionedDirectory> Subdirectories { get; }
    public IEnumerable<PermissionedFile> Files { get; }

    // Para compatibilidad con Consumers legacy de Children
    public IEnumerable<INamed> Children 
        => Subdirectories.Cast<INamed>().Concat(Files);

    internal PermissionedDirectory(
        string name,
        UnixPermissions permissions,
        IEnumerable<PermissionedDirectory> subdirs,
        IEnumerable<PermissionedFile> files)
    {
        Name           = name;
        Permissions    = permissions;
        Subdirectories = subdirs;
        Files          = files;
    }
}

public class PermissionedFile : INamedByteSource, IPermissioned
{
    private readonly INamedByteSource inner;
    public string Name => inner.Name;
    public UnixPermissions Permissions { get; }

    public PermissionedFile(INamedByteSource inner, UnixPermissions perms)
    {
        this.inner      = inner;
        Permissions = perms;
    }

    public IObservable<byte[]> Bytes
        => Permissions.OwnerRead 
           ? inner.Bytes 
           : Observable.Empty<byte[]>();

    public IDisposable Subscribe(IObserver<byte[]> observer)
    {
        return inner.Subscribe(observer);
    }
}

// 2. Fábrica recursiva: aquí viven los resolvers
public static class PermissionedTreeBuilder
{
    public static PermissionedDirectory Build(
        IDirectory root,
        Func<IDirectory, UnixPermissions> dirResolver,
        Func<INamedByteSource, UnixPermissions> fileResolver)
    {
        return BuildDirectory(root, dirResolver, fileResolver);
    }

    private static PermissionedDirectory BuildDirectory(
        IDirectory dir,
        Func<IDirectory, UnixPermissions> dirResolver,
        Func<INamedByteSource, UnixPermissions> fileResolver)
    {
        var perms = dirResolver(dir);

        // Generamos el subárbol de directorios antes de los ficheros
        var subdirs = dir
            .Subdirectories
            .Select(d => BuildDirectory(d, dirResolver, fileResolver));

        var files = dir
            .Files
            .Select(f => new PermissionedFile(f, fileResolver(f)));

        return new PermissionedDirectory(
            dir.Name,
            perms,
            subdirs,
            files);
    }
}