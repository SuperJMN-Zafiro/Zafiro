using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace FileSystem.Tests;

public class CacheTests
{
    [Fact]
    public async Task File_is_stored()
    {
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>());
        var sut = await StreamCache.CreateInstance(fs, @"C:\");

        await sut.Store("C:\\File.txt", () => Content("Hi"));

        var file = fs.GetFile("C:\\File.txt");
        file.TextContents.Should().Be("Hi");
    }

    private static async Task<StreamCache> CreateSut(MockFileSystem fs)
    {
        return await StreamCache.CreateInstance(fs, @"C:\");
    }

    [Fact]
    public async Task Same_contents_should_be_a_cache_hit()
    {
        var cacheTester = new CacheFixture();
        await cacheTester.StoreSequence("Hi", "Hi");
        cacheTester.OpenedStreams.Should().Be(1);
    }

    [Fact]
    public async Task Different_contents_should_be_a_cache_fault()
    {
        var cacheTester = new CacheFixture();
        await cacheTester.StoreSequence("Hi", "Different");
        cacheTester.OpenedStreams.Should().Be(2);
    }

    private static MemoryStream Content(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }
}