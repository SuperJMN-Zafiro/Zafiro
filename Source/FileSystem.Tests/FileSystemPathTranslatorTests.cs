using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using Xunit;

namespace FileSystem.Tests;

public class FileSystemPathTranslatorTests
{
    [Theory]
    [InlineData(@"C:\Dir\Subdir\File.txt", @"C:\Dir", @"C:\home\pi", @"C:\home\pi\Subdir\File.txt")]
    [InlineData(@"C:\Dir\Subdir", @"C:\Dir", @"C:\home\pi", @"C:\home\pi\Subdir")]
    [InlineData(@"C:\File.txt", @"C:\", @"C:\home", @"C:\home\File.txt")]
    public void Translate(string path, string origin, string destination, string expectedPath)
    {
        var sut = new FileSystemPathTranslator();
        var originFs = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [path] = MockFileData.NullObject
        });

        var linuxFs = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [destination] = new MockDirectoryData()
        });

        var translated = sut.Translate(
            originFs.FileInfo.FromFileName(path),
            originFs.DirectoryInfo.FromDirectoryName(origin),
            linuxFs.DirectoryInfo.FromDirectoryName(destination));

        translated.Should().Be(expectedPath);
    }
}