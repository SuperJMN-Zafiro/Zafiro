using System.IO.Abstractions.TestingHelpers;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Local;

namespace Zafiro.FileSystem.Tests.Old;

public class CopyDirectoryActionTests
{
    [Fact]
    public async Task Executing_CopyDirectoryAction_should_copy_directory_recursively()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["C:\\Source\\File1.txt"] = new("Hi"),
            ["C:\\Source\\File2.txt"] = new("How"),
            ["C:\\Source\\Subfolder\\File3.txt"] = new("Are you?"),
        });
        var fs = new LocalFileSystem(mockFileSystem, Maybe<ILogger>.None);
        var action = from src in fs.GetDirectory("C:\\Source")
                     from dst in fs.GetDirectory("C:\\Destination")
                     from ca in CopyDirectoryAction.Create(src, dst)
                     select ca;

        var result = await action.Bind(directoryAction => directoryAction.Execute(CancellationToken.None));

        result.Should().Succeed();
        mockFileSystem.GetFile("C:\\Destination\\File1.txt").TextContents.Should().Be("Hi");
        mockFileSystem.GetFile("C:\\Destination\\File2.txt").TextContents.Should().Be("How");
        mockFileSystem.GetFile("C:\\Destination\\Subfolder\\File3.txt").TextContents.Should().Be("Are you?");
    }
}