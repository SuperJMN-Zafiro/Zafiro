using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using CSharpFunctionalExtensions;
using DynamicData;
using Xunit;
using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.SeaweedFS.Tests;

public class DirectoryTests
{
    [Theory]
    [InlineData("Juegos/ROMs")]
    [InlineData("Juegos")]
    public async Task From_should_succeed(string path)
    {
        var seaweedFSClient = SutFactory.Create();
        var result = await Directory.From(path, seaweedFSClient);
        result.Should().Succeed();
    }
    
    [Theory]
    [InlineData("Juegos/ROMs")]
    [InlineData("Juegos/ROMs/Gameboy")]
    [InlineData("Juegos/ROMs/3DS")]
    [InlineData("Juegos")]
    public async Task Children_should_succeed(string path)
    {
        var seaweedFSClient = SutFactory.Create();
        var result = await Directory.From(path, seaweedFSClient).Bind(x => x.GetChildren());
        result.Should().Succeed().And.Subject.Value.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task Directory_contents()
    {
        var result = await Directory.From("file", SutFactory.Create()).Bind(directory => directory.GetChildren());
        result.Should().Succeed().And.Subject.Value.Should().NotBeEmpty();
    }
}