using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zafiro.FileSystem;

namespace FileSystem.Tests;

public class FileSystemMixinTests
{
    [Fact]
    public async Task Copy_between_filesystems_should_produce_correct_paths()
    {
        var originFs = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            ["Dir\\File1.txt"] = MockFileData.NullObject,
            ["Dir\\Subdir\\File1.txt"] = MockFileData.NullObject
        });
        var destinationFs = new MockFileSystem();
        var origin = new FileSystemPath(originFs, "Dir");
        var destination = new FileSystemPath(destinationFs, "New");

        await origin.CopyTo(destination).ConfigureAwait(false);

        var expectedPaths = new[]
        {
            destinationFs.FileInfo.FromFileName(@"New\File1.txt"),
            destinationFs.FileInfo.FromFileName(@"New\Subdir\File1.txt")
        }.Select(f => f.FullName);

        destinationFs.AllFiles.Should().BeEquivalentTo(expectedPaths);
    }
}