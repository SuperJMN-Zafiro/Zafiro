using System.Text;
using Zafiro.DivineBytes.Permissioned;

namespace Zafiro.DivineBytes.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var f1 = new File("script.sh", ByteSource.FromString("#!/bin/bash", Encoding.Default));
        var f2 = new File("readme.txt", ByteSource.FromString("Hola mundo", Encoding.Default));
        var f3 = new File("data.bin",   ByteSource.FromString("010101", Encoding.Default));

        // 2. Directorios: /logs contiene f1, /docs contiene f2, raíz contiene ambos y f3
        var logs = new Directory("logs", f1);
        var docs = new Directory("docs", f2);
        var root = new Directory("root", logs, docs, f3);
        
        Func<IDirectory, UnixPermissions> dirPerm = d
            => new UnixPermissions(
                ownerRead: true, ownerWrite: true, ownerExec: true,
                groupRead: true, groupWrite: false, groupExec: true,
                otherRead: true, otherWrite: false, otherExec: true);
        
        Func<INamedByteSource, UnixPermissions> filePerm = f =>
            f.Name.EndsWith(".sh")
                ? new UnixPermissions( true, false, true,  true, false, true,  true, false, true )
                : f.Name.EndsWith(".txt")
                    ? new UnixPermissions( true, true,  false, true, false, false, true, false, false )
                    : new UnixPermissions(  true, false, false, false, false, false, false, false, false );
        
        var secureRoot = PermissionedTreeBuilder.Build(root, dirPerm, filePerm);
    }
}