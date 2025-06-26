using System.Text;
using Zafiro.DivineBytes.Permissioned;

namespace Zafiro.DivineBytes.Tests;

public class UnitTest1
{
    private readonly IMetadataResolver resolver = new TestMetadataResolver();
    private readonly UnixTreeBuilder builder;

    public UnitTest1()
    {
        builder = new UnixTreeBuilder(resolver);
    }
    
    [Fact]
    public void Test1()
    {
        // Arrange
        var f1 = new File("script.sh", ByteSource.FromString("#!/bin/bash", Encoding.Default));
        var f2 = new File("readme.txt", ByteSource.FromString("Hola mundo", Encoding.Default));
        var f3 = new File("data.bin",   ByteSource.FromString("010101",     Encoding.Default));

        var logs = new Directory("logs", f1);
        var docs = new Directory("docs", f2);
        var root = new Directory("root", logs, docs, f3);

        // Act
        var secureRoot = builder.Build(root);

        // Assert: directorio raíz
        Assert.Equal("root", secureRoot.Name);
        Assert.True(secureRoot.Permissions.OwnerRead);
        Assert.False(secureRoot.Permissions.GroupWrite);
        Assert.Equal(0, secureRoot.OwnerId);

        // Assert: subdirectorio logs y su fichero
        var logsDir = secureRoot.Subdirectories.Single(d => d.Name == "logs");
        Assert.True(logsDir.Permissions.OwnerExec);
        var script = logsDir.Files.Single(f => f.Name == "script.sh");
        Assert.True(script.Permissions.OwnerExec);

        // Assert: subdirectorio docs y su fichero
        var docsDir = secureRoot.Subdirectories.Single(d => d.Name == "docs");
        var readme  = docsDir.Files.Single(f => f.Name == "readme.txt");
        Assert.True(readme.Permissions.OwnerWrite);
        Assert.False(readme.Permissions.OwnerExec);

        // Assert: fichero suelto data.bin
        var data = secureRoot.Files.Single(f => f.Name == "data.bin");
        Assert.True(data.Permissions.OwnerRead);
        Assert.False(data.Permissions.OwnerWrite);
    }
}

public class TestMetadataResolver : IMetadataResolver
{
    public Metadata ResolveDirectory(IDirectory dir)
        => new(
            new UnixPermissions(
                ownerRead:  true, ownerWrite: true, ownerExec:  true,
                groupRead:  true, groupWrite: false, groupExec:  true,
                otherRead:  true, otherWrite: false, otherExec:  true),
            OwnerId: 0);

    public Metadata ResolveFile(INamedByteSource file)
    {
        if (file.Name.EndsWith(".sh"))
            return new(
                new UnixPermissions(
                    ownerRead:  true, ownerWrite: false, ownerExec:  true,
                    groupRead:  true, groupWrite: false, groupExec:  true,
                    otherRead:  true, otherWrite: false, otherExec:  true),
                OwnerId: 0);

        if (file.Name.EndsWith(".txt"))
            return new(
                new UnixPermissions(
                    ownerRead:  true, ownerWrite: true, ownerExec:  false,
                    groupRead:  true, groupWrite: false, groupExec:  false,
                    otherRead:  true, otherWrite: false, otherExec:  false),
                OwnerId: 0);

        return new(
            new UnixPermissions(
                ownerRead:  true, ownerWrite: false, ownerExec:  false,
                groupRead:  false, groupWrite: false, groupExec:  false,
                otherRead:  false, otherWrite: false, otherExec:  false),
            OwnerId: 0);
    }
}