using CSharpFunctionalExtensions;
using System.IO;

namespace Zafiro.FileSystem.Tests.Old;

public class PathTests
{
    [Fact]
    public void Valid_path()
    {
        ZafiroPath.Create("This is a path").Should().Succeed();
    }

    [Fact]
    public void Valid_path_with_subparts()
    {
        ZafiroPath.Create("This is a path/sub dir").Should().Succeed();
    }

    [Fact]
    public void Invalid_path()
    {
        ZafiroPath.Create("/").Should().Fail();
    }

    [Fact]
    public void Empty_path()
    {
        var path = ZafiroPath.Create("");
        path.Should().Fail();
    }

    [Fact]
    public void Empty_path_multiple_spaces()
    {
        var path = ZafiroPath.Create("   ");
        path.Should().Fail();
    }

    [Fact]
    public void Parents_of_file()
    {
        var zafiroPath = ZafiroPath.Create("C:/Users/JMN/Desktop/File.txt");
        var parents = zafiroPath
            .Map(path => path.Parents());

        var zafiroPaths = new ZafiroPath[]
        {
            new("C:"),
            new("C:/Users"),
            new("C:/Users/JMN"),
            new("C:/Users/JMN/Desktop")
        };

        parents.Should().Succeed().And.Subject.Value.Should().BeEquivalentTo(zafiroPaths);
    }

    [Fact]
    public void Parents_of_root()
    {
        var zafiroPath = ZafiroPath.Empty;
        var parents = zafiroPath.Parents();

        var zafiroPaths = new ZafiroPath[] { };

        parents.Should().BeEquivalentTo(zafiroPaths);
    }

    [Fact]
    public void Parents_single()
    {
        var zafiroPath = ZafiroPath.Create("Desktop");
        var parents = zafiroPath
            .Map(path => path.Parents());

        var zafiroPaths = new ZafiroPath[]
        {
        };

        parents.Should().Succeed().And.Subject.Value.Should().BeEquivalentTo(zafiroPaths);
    }
}