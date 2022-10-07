using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using Xunit;

namespace Zafiro.SftpFileSystem.Tests;

public class PathTests
{
    [Theory]
    [InlineData(@"/Program Files/Adobe/Photoshop 2020", @"/Program Files/SuperJMN", @"../../SuperJMN")]
    [InlineData(@"/Program Files/Photoshop 2020", @"/Program Files", "..")]
    [InlineData(@"/Program Files/Photoshop 2020", @"/Users/JMN", @"../../Users/JMN")]
    [InlineData(@"/Repos/SmartSftpCopy/SmartSftpCopy/bin/Debug/net6.0/hashes.dat", @"/Users/JMN/Downloads",
        @"../../../../../../../Users/JMN/Downloads")]
    public void Test1(string root, string path, string expected)
    {
        var sut = new Path(new MockFileSystem(new Dictionary<string, MockFileData>()));
        var actual = sut.GetRelativePath(root, path);
        actual.Should().Be(expected);
    }
}