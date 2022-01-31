using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace FileSystem.Tests;

public class FilesystemComparerTests
{
    [Theory]
    [ClassData(typeof(TestCase))]
    public async Task Diff(IFileSystem source, IFileSystem destination, FileDiff[] diff)
    {
        var f = new Mock<IFileSystemPathTranslator>();
        f.Setup(r => r.Translate(It.IsAny<IFileSystemInfo>(), It.IsAny<IFileSystemInfo>(), It.IsAny<IDirectoryInfo>()))
            .Returns((IFileSystemInfo a, IFileSystemInfo b, IDirectoryInfo _) => a.FullName);


        var sut = new FileSystemComparer(f.Object, new FileComparer());
        var results = await sut.Diff(source.DirectoryInfo.FromDirectoryName("."),
            destination.DirectoryInfo.FromDirectoryName("."));
        results.Should().BeEquivalentTo(diff);
    }

    private class TestCase : TheoryData<IFileSystem, IFileSystem, FileDiff[]>
    {
        public TestCase()
        {
            var sourceFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                ["File.txt"] = MockFileData.NullObject
            });
            var result = new FileDiff(sourceFileSystem.FileInfo.FromFileName("File.txt"),
                FileDiffStatus.Deleted);
            Add(sourceFileSystem, new MockFileSystem(new Dictionary<string, MockFileData>()), new[] {result});
            Add(new MockFileSystem(), new MockFileSystem(), Array.Empty<FileDiff>());
            Add(
                Fs(new Dictionary<string, MockFileData>
                {
                    ["File.txt"] = MockFileData.NullObject
                }),
                Fs(),
                new[]
                {
                    ("File.txt", FileDiffStatus.Deleted)
                });
        }

        private static MockFileSystem Fs()
        {
            return new MockFileSystem();
        }

        private static MockFileSystem Fs(Dictionary<string, MockFileData> mockFileDatas)
        {
            return new MockFileSystem(mockFileDatas);
        }

        private void Add(IFileSystem origin, IFileSystem destination,
            IEnumerable<(string, FileDiffStatus)> diffs)
        {
            AddRow(origin, destination,
                diffs.Select(tuple => new FileDiff(origin.FileInfo.FromFileName(tuple.Item1), tuple.Item2)));
        }
    }
}