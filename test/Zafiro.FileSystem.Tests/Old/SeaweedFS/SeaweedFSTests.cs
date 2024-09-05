using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Moq;
using Zafiro.FileSystem;

namespace Zafiro.FileSystem.Tests.Old.SeaweedFS;

public class SeaweedFSTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Exists_should_match(bool match)
    {
        var p = Mock.Of<ISeaweedFS>(w => w.PathExists("path") == Task.FromResult(match));
        var seaweedFS = new SeaweedFileSystem(p, Maybe<ILogger>.None);
        var result = await ZafiroPath
            .Create("path")
            .Bind(seaweedFS.GetFile)
            .Bind(file => file.Exists());

        result.Should().Succeed().And.Subject.Value.Should().Be(match);
    }
}