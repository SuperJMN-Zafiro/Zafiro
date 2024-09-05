using System.IO.Abstractions.TestingHelpers;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Evolution;

namespace Zafiro.FileSystem.Tests.Old.Local;

public class FileSystemRootTests
{
    [Fact]
    public async Task Tested()
    {
        var mfs = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["Test.txt"] = new("asdf"),
        });
        var sut = new FileSystemRoot(new ObservableFileSystem(new LocalFileSystem2(mfs)));
        var dir = sut.GetDirectory("NewDir");

        await dir.Exists.TapIf(b => !b, () => dir.Create());

        mfs.DirectoryInfo.New("NewDir").Exists.Should().BeTrue();
    }
}