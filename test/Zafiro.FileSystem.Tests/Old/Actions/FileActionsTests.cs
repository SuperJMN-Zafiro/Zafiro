using System.IO.Abstractions.TestingHelpers;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Local;
using Zafiro.FileSystem.Evolution.Synchronizer;

namespace Zafiro.FileSystem.Tests.Old.Actions;

public class FileActionsTests
{
    [Fact]
    public async Task CopyLeftFilesToRightSideAction_syncs_OK()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["C:\\Source\\File1.txt"] = new("Hi"),
            ["C:\\Source\\File2.txt"] = new("How"),
            ["C:\\Destination\\File1.txt"] = new("Heya!")
        });

        var local = new LocalFileSystem(mockFileSystem, Maybe<ILogger>.None);
        var r = await local.GetDirectory("C:/Source").CombineAndBind(local.GetDirectory("C:/Destination"), CopyLeftFilesToRightSideAction.Create);
        var execution = await r.Bind(s => s.Execute(CancellationToken.None));
        execution.Should().Succeed();

        mockFileSystem.GetFile("C:\\Source\\File1.txt").TextContents.Should().Be("Hi");
        mockFileSystem.GetFile("C:\\Source\\File2.txt").TextContents.Should().Be("How");
        mockFileSystem.GetFile("C:\\Destination\\File1.txt").TextContents.Should().Be("Heya!");
        mockFileSystem.GetFile("C:\\Destination\\File2.txt").TextContents.Should().Be("How");
    }

    [Fact]
    public async Task OverwriteRightFilesInBothSides_syncs_OK()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["C:\\Source\\File1.txt"] = new("Hi"),
            ["C:\\Source\\File2.txt"] = new("How"),
            ["C:\\Destination\\File1.txt"] = new("Heya!")
        });

        var local = new LocalFileSystem(mockFileSystem, Maybe<ILogger>.None);
        var r = await local.GetDirectory("C:/Source").CombineAndBind(local.GetDirectory("C:/Destination"), OverwriteRightFilesInBothSides.Create);
        var execution = await r.Bind(s => s.Execute(CancellationToken.None));
        execution.Should().Succeed();

        mockFileSystem.GetFile("C:\\Source\\File1.txt").TextContents.Should().Be("Hi");
        mockFileSystem.GetFile("C:\\Source\\File2.txt").TextContents.Should().Be("How");
        mockFileSystem.GetFile("C:\\Destination\\File1.txt").TextContents.Should().Be("Hi");
        mockFileSystem.GetFile("C:\\Destination\\File2.txt").Should().BeNull();
    }

    [Fact]
    public async Task DeleteFilesOnlyOnRightSide_syncs_OK()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["C:\\Source\\File1.txt"] = new("Hi"),
            ["C:\\Source\\File2.txt"] = new("How"),
            ["C:\\Destination\\File1.txt"] = new("1"),
            ["C:\\Destination\\File2.txt"] = new("2"),
            ["C:\\Destination\\File3.txt"] = new("3"),
        });

        var local = new LocalFileSystem(mockFileSystem, Maybe<ILogger>.None);
        var r = await local.GetDirectory("C:/Source").CombineAndBind(local.GetDirectory("C:/Destination"), DeleteFilesOnlyOnRightSide.Create);
        var execution = await r.Bind(s => s.Execute(CancellationToken.None));
        execution.Should().Succeed();

        mockFileSystem.GetFile("C:\\Source\\File1.txt").TextContents.Should().Be("Hi");
        mockFileSystem.GetFile("C:\\Source\\File2.txt").TextContents.Should().Be("How");
        mockFileSystem.GetFile("C:\\Destination\\File1.txt").TextContents.Should().Be("1");
        mockFileSystem.GetFile("C:\\Destination\\File2.txt").TextContents.Should().Be("2");
        mockFileSystem.GetFile("C:\\Destination\\File3.txt").Should().BeNull();
    }
}