using CSharpFunctionalExtensions;
using Xunit;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.FileSystem.SeaweedFS.Tests;

public class FileTests
{
    [Theory]
    [InlineData("Juegos/ROMs/Gameboy/Atomic Punk (USA).zip")]
    [InlineData("file.txt")]
    public async Task Retrieving_directory_should_succeed(string path)
    {
        var seaweedFSClient = SutFactory.Create();
        var result = (Result<File>)(File)new File(path, seaweedFSClient);
        result.Should().Succeed();
    }
    
    [Fact]
    public async Task Retrieving_inexistent_fails()
    {
        var seaweedFSClient = SutFactory.Create();
        var result = (Result<File>)(File)new File("Inexistent", seaweedFSClient);
        result.Should().Fail();
    }
    
    [Fact]
    public async Task Inexistent_file_should_not_exist()
    {
        var sut = new SeaweedFS.File("Inexistent.txt", SutFactory.Create());
        var result = await sut.Exists();
        result.Should().SucceedWith(false);
    }
    
    [Fact]
    public async Task Existent_file_should_exist()
    {
        var sut = new File("file.txt", SutFactory.Create());
        var result = await sut.Exists();
        result.Should().SucceedWith(true);
    }
    
    [Fact]
    public async Task Content_lenght_should_be_greater_than_zero()
    {
        var sut = new File("file.txt", SutFactory.Create());
        var result = await sut.GetContents();
        result.Should().Succeed().And.Subject.Value.Length.Should().BeGreaterThan(0);
    }
}