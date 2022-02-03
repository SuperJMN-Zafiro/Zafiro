using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Xunit;

namespace FileSystem.Tests;

public class SmartFileManagerTests
{
    [Fact]
    public async Task Copying_same_file_twice_writes_it_once()
    {
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["file1.txt"] = new("A")
        });

        var sut = new SmartFileManager("TestingFS");

        var origin = fs.FileInfo.FromFileName("file1.txt");
        var destination = fs.FileInfo.FromFileName("New\\file1.txt");

        await sut.Copy(origin, destination);
        await sut.Copy(origin, destination);
    }
}