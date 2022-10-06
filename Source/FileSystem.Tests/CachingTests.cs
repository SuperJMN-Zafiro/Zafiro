using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FileSystem.Caching;
using Serilog;
using Xunit;

namespace FileSystem.Tests;

public class CachingTests
{
    [Fact]
    public async Task Test()
    {
        var sut = new CachingZafiroFileSystem(new ZafiroFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None));
        var file1 = await sut.GetFile("D:\\Telegram\\documents\\IMG_20220414_194740.jpg").Value.OpenRead();
        var file2 = await sut.GetFile("D:\\Telegram\\documents\\IMG_20220414_194740.jpg").Value.OpenRead();
    }
}