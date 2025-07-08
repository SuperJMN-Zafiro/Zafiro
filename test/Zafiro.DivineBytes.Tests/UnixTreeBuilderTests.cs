using System.Text;
using Zafiro.DivineBytes.Unix;

namespace Zafiro.DivineBytes.Tests;

public class UnixTreeBuilderTests
{
    private readonly IMetadataResolver resolver = new TestMetadataResolver();
    private readonly UnixTreeBuilder builder;

    public UnixTreeBuilderTests()
    {
        builder = new UnixTreeBuilder(resolver);
    }
    
    [Fact]
    public void Test1()
    {
        // Arrange
        var f1 = new Resource("script.sh", ByteSource.FromString("#!/bin/bash", Encoding.Default));
        var f2 = new Resource("readme.txt", ByteSource.FromString("Hola mundo", Encoding.Default));
        var f3 = new Resource("data.bin",   ByteSource.FromString("010101",     Encoding.Default));

        var logs = new Container("logs", f1);
        var docs = new Container("docs", f2);
        var root = new Container("root", logs, docs, f3);

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