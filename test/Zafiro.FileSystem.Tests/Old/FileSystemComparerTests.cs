using System.IO.Abstractions.TestingHelpers;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.Comparer;
using Zafiro.FileSystem.Local;

namespace Zafiro.FileSystem.Tests.Old;

public class FileSystemComparerTests
{
    [Fact]
    public async Task Test()
    {
        var fileSystem1 = new LocalFileSystem(new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            [@"C:\Dir1\File1.txt"] = new("Sample"),
            [@"C:\Dir1\File2.txt"] = new("Sample"),
            [@"C:\Dir2\File1.txt"] = new("Sample"),
            [@"C:\Dir3\File1.txt"] = new("Sample"),
        }), Maybe<ILogger>.None);

        var fileSystem2 = new LocalFileSystem(new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            [@"D:\Dir1\File1.txt"] = new("Sample"),
            [@"D:\Dir1\File3.txt"] = new("Sample"),
            [@"D:\Dir2\File1.txt"] = new("Sample"),
            [@"D:\Dir2\File4.txt"] = new("Sample"),
        }), Maybe<ILogger>.None);

        var sut = new FileSystemComparer();

        var diff = await fileSystem1.GetDirectory("C:/")
            .Bind(source => fileSystem2.GetDirectory("D:/")
                .Bind(dest => sut.Diff(source, dest)));

        diff.Should().Succeed().And.Subject.Value.Should().NotBeEmpty();
    }
}

