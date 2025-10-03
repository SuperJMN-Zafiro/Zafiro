﻿using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Serilog;
using Xunit;
using Zafiro.FileSystem;

namespace FileSystem.Tests;

public class CopierTests
{
    [Fact]
    public async Task Everything_is_added()
    {
        var innerFs = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [@"C:\Subdir\Root.txt"] = new("A"),
            [@"C:\Subdir\Data\Something1.txt"] = new("B"),
            [@"C:\Subdir\Data\Something2.txt"] = new("C"),
            [@"C:\Subdir\Data\InnerMost\Something1.txt"] = new("D")
        });
        var fs = new ZafiroFileSystem(innerFs, Maybe<ILogger>.None);

        var result = await Copy(fs, "C:>Subdir", "C:>Destination").ConfigureAwait(false);
        result.IsSuccess.Should().BeTrue();

        RelativeFlatFileList(innerFs.DirectoryInfo.FromDirectoryName(@"C:\Subdir"))
            .Should()
            .BeEquivalentTo(RelativeFlatFileList(innerFs.DirectoryInfo.FromDirectoryName(@"C:\Destination")));
    }

    [Fact]
    public async Task Existing_destination_is_replaced()
    {
        var innerFs = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [@"C:\Subdir\Root.txt"] = new("A"),
            [@"C:\Destination\Root.txt"] = new("B")
        });

        var fs = new ZafiroFileSystem(innerFs, Maybe<ILogger>.None);

        var result = await Copy(fs, @"C:>Subdir", "C:>Destination").ConfigureAwait(false);

        result.IsSuccess.Should().BeTrue();
        innerFs.GetFile(@"C:\Destination\Root.txt").TextContents.Should().Be("A");
    }

    [Fact]
    public async Task Non_existing_file_is_deleted()
    {
        var innerFs = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [@"C:\Destination\Root.txt"] = new("B")
        });

        var fs = new ZafiroFileSystem(innerFs, Maybe<ILogger>.None);

        var result = await Copy(fs, @"C:>Subdir", "C:>Destination").ConfigureAwait(false);
        result.IsSuccess.Should().BeTrue();
        innerFs.GetFile(@"C:\Destination\Root.txt").Should().BeNull();
    }

    private static async Task<Result> Copy(IZafiroFileSystem fs, string origin, string destination)
    {
        var copy = from o in fs.GetDirectory(origin)
            from d in fs.GetDirectory(destination)
            select new {o, d};

        var sut = CreateSut();
        var result = await copy.Bind(r => sut.Sync( r.o, r.d)).ConfigureAwait(false);
        return result;
    }

    private static IEnumerable<string> RelativeFlatFileList(IDirectoryInfo root)
    {
        return root
            .GetFiles("*", SearchOption.AllDirectories)
            .Select(r => root.GetRelativePath(r.FullName));
    }

    private static Syncer CreateSut()
    {
        var fileSystemComparer = new ZafiroFileSystemComparer();
        var sut = new Syncer(fileSystemComparer);
        return sut;
    }
}