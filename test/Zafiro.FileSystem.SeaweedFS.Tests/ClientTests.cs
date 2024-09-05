using Xunit;

namespace Zafiro.FileSystem.SeaweedFS.Tests;

public class ClientTests
{
    [Theory]
    [InlineData("Juegos/ROMs/Gameboy/Atomic Punk (USA).zip")]
    [InlineData("file.txt")]
    public async Task Retrieving_metadata_should_succeed(string path)
    {
        await AssertMetadata(path);
    }
    
    [Theory]
    [InlineData("Juegos/ROMs")]
    [InlineData("Juegos")]
    public async Task Retrieving_directory_should_succeed(string path)
    {
        await AssertGetDirectory(path);
    }

    private async Task AssertGetDirectory(string path)
    {
        var sut = SutFactory.Create();
        var result = await sut.GetContents(path);
        result.Should().Succeed();
    }

    private static async Task AssertMetadata(string path)
    {
        var sut = SutFactory.Create();
        var result = await sut.GetFileMetadata(path);
        result.Should().Succeed();
    }
}