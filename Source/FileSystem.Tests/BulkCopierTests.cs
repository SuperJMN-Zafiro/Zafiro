using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace FileSystem.Tests;

public class BulkCopierTests
{
    [Fact]
    public async Task Everything_is_added()
    {
        var sut = CreateSut();
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [@"C:\Subdir\Root.txt"] = new("A"),
            [@"C:\Subdir\Data\Something1.txt"] = new("B"),
            [@"C:\Subdir\Data\Something2.txt"] = new("C"),
            [@"C:\Subdir\Data\InnerMost\Something1.txt"] = new("D")
        });

        var origin = fs.DirectoryInfo.FromDirectoryName(@"C:\Subdir");
        var destination = fs.DirectoryInfo.FromDirectoryName(@"C:\Destination");
        await sut.Copy(origin, destination);

        RelativeFlatFileList(origin)
            .Should()
            .BeEquivalentTo(RelativeFlatFileList(destination));
    }

    [Fact]
    public async Task Existing_destination_is_replaced()
    {
        var sut = CreateSut();
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [@"C:\Subdir\Root.txt"] = new("A"),
            [@"C:\Destination\Root.txt"] = new("B")
        });

        await sut.Copy(fs.DirectoryInfo.FromDirectoryName(@"C:\Subdir"),
            fs.DirectoryInfo.FromDirectoryName(@"C:\Destination"));

        fs.GetFile(@"C:\Destination\Root.txt").TextContents.Should().Be("B");
    }

    [Fact]
    public async Task Non_existing_file_is_deleted()
    {
        var sut = CreateSut();
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [@"C:\Destination\Root.txt"] = new("B")
        });

        await sut.Copy(fs.DirectoryInfo.FromDirectoryName(@"C:\Subdir"),
            fs.DirectoryInfo.FromDirectoryName(@"C:\Destination"));

        fs.GetFile(@"C:\Destination\Root.txt").Should().BeNull();
    }

    private static IEnumerable<string> RelativeFlatFileList(IDirectoryInfo root)
    {
        return root
            .GetFiles("*", SearchOption.AllDirectories)
            .Select(r => root.GetRelativePath(r.FullName));
    }

    private static BulkCopier CreateSut()
    {
        var pathTranslator = new FileSystemPathTranslator();
        var fileSystemComparer = new FileSystemComparer(pathTranslator, new FileComparer());
        var sut = new BulkCopier(fileSystemComparer, pathTranslator);
        return sut;
    }
}