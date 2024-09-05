using System.IO.Abstractions.TestingHelpers;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Local;

namespace Zafiro.FileSystem.Tests.Old;

public class ZafiroDirectoryMixinTests
{
    [Fact]
    public async Task Maybe_file()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["C:\\Dir1\\Dir2\\File.txt"] = new(""),
        });

        var fileSystem = new LocalFileSystem(mockFileSystem, Maybe<ILogger>.None);
        var result = await fileSystem.GetDirectory("C:/Dir1")
            .Bind(directory => directory.DescendantFile("Dir2/File.txt"));

        result.Should().Succeed().And.Subject.Value.Value.Should().BeAssignableTo<IZafiroFile>();
    }

    [Fact]
    public async Task Maybe_directory()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["C:\\Dir1\\Dir2\\File.txt"] = new(""),
        });

        var fileSystem = new LocalFileSystem(mockFileSystem, Maybe<ILogger>.None);
        var result = await fileSystem.GetDirectory("C:/Dir1")
            .Bind(directory => directory.DescendantDirectory("Dir2"));

        result.Should().Succeed().And.Subject.Value.Value.Should().BeAssignableTo<IZafiroDirectory>();
    }
}