using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Xunit;

namespace FileSystem.Tests;

public class SmartFileManagerTests
{
    [Fact]
    public async Task Test()
    {
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["file1.txt"] = new("A")
        });

        var file = fs.FileInfo.FromFileName("C:\\hashes.dat");
        var sut = new SmartFileManager(file);

        await sut.Copy(fs.FileInfo.FromFileName("file1.txt"), fs.FileInfo.FromFileName("New\\file1.txt"));
        sut.Delete(fs.FileInfo.FromFileName("New\\file1.txt"));
        await sut.Copy(fs.FileInfo.FromFileName("file1.txt"), fs.FileInfo.FromFileName("New\\file1.txt"));
        await sut.Save();
    }
}