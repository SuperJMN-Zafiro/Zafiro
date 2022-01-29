using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace FileSystem.Tests;

public class CacheTests
{
    [Fact]
    public async Task Contains_with_same_data_should_be_true()
    {
        var fs = new MockFileSystem();
        var sut = new Cache(fs, "C:\\Data", new HashGenerator());
        await sut.Add(@"sample.txt", () => CreateContent("Hi"));
        var contains = await sut.Contains(@"sample.txt", () => CreateContent("Hi"));
        contains.Should().BeTrue();
    }

    [Fact]
    public async Task Contains_with_different_data_should_be_false()
    {
        var fs = new MockFileSystem();
        var sut = new Cache(fs, "C:\\Data", new HashGenerator());
        await sut.Add(@"sample.txt", () => CreateContent("Hi"));
        var contains = await sut.Contains(@"sample.txt", () => CreateContent("Ouh!"));
        contains.Should().BeFalse();
    }

    [Fact]
    public async Task Cache_contents_from_file_should_exist()
    {
        var hashGenerator = new HashGenerator();

        Cache sut;

        var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            [@"C:\Data\contents.hash"] = new(System.Text.Json.JsonSerializer.Serialize(new Dictionary<string, byte[]>()
            {
                [@"sample.txt"] = await hashGenerator.ComputeHash(() => CreateContent("Hi")),
            }))
        });

        sut = new Cache(fs, "C:\\Data", hashGenerator);
        var contains = await sut.Contains(@"sample.txt", () => CreateContent("Hi"));
        contains.Should().BeTrue();
    }

    private static Stream CreateContent(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }
}