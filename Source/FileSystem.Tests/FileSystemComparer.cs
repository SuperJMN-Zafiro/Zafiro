using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MoreLinq;
using MoreLinq.Extensions;
using Xunit;

namespace FileSystem.Tests;

public class FilesystemComparerTests
{
    [Theory]
    [ClassData(typeof(TestCase))]
    public async Task Diff(IFileSystem source, IFileSystem destination, FileDiff[] diff)
    {
        var f = new Mock<IFileSystemPathTranslator>();
        f.Setup(r => r.Translate(It.IsAny<IFileSystemInfo>(), It.IsAny<IDirectoryInfo>()))
            .Returns((IFileSystemInfo a, IDirectoryInfo _) => a.FullName);


        var sut = new FileSystemComparer(f.Object, new FileComparer());
        var results = await sut.Comparer(source.DirectoryInfo.FromDirectoryName("."),
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
            var result = new FileDiff(sourceFileSystem.FileInfo.FromFileName("File.txt").FullName,
                FileDiffStatus.Deleted);
            Add(sourceFileSystem, new MockFileSystem(new Dictionary<string, MockFileData>()), new[] {result});
            Add(new MockFileSystem(), new MockFileSystem(), Array.Empty<FileDiff>());
        }
    }
}

public class FileSystemPathTranslator : IFileSystemPathTranslator
{
    public string Translate(IFileSystemInfo fileSystemInfo, IDirectoryInfo destination)
    {
        throw new NotImplementedException();
    }
}

public interface IFileSystemPathTranslator
{
    public string Translate(IFileSystemInfo fileSystemInfo, IDirectoryInfo destination);
}

public class FileComparer : IFileComparer
{
    public bool AreEqual(IFileInfo info, IFileInfo fileInfo)
    {
        throw new NotImplementedException();
    }
}

public interface IFileComparer
{
    bool AreEqual(IFileInfo info, IFileInfo fileInfo);
}

public enum FileDiffStatus
{
    Invalid = 0,
    Deleted,
    Unchanged,
    Modified,
    Created
}

public class FileSystemComparer
{
    private readonly IFileComparer fileComparer;
    private readonly IFileSystemPathTranslator pathTranslator;

    public FileSystemComparer(IFileSystemPathTranslator pathTranslator, IFileComparer fileComparer)
    {
        this.pathTranslator = pathTranslator;
        this.fileComparer = fileComparer;
    }

    public async Task<IEnumerable<FileDiff>> Comparer(IDirectoryInfo origin, IDirectoryInfo destination)
    {
        var originFiles = GetFilesRecursively(origin);
        var destinationFiles = GetFilesRecursively(destination);

        //return originFiles.FullJoin<IFileInfo, string, FileDiff>(destinationFiles, f => pathTranslator.Translate(f, destination), f => f.FullName,
        //    f => f.FullName, (destinationFiles, b) => GenerateResult(destinationFiles, b));

        return FullJoinExtension.FullJoin(originFiles, destinationFiles,
            f => pathTranslator.Translate(f, destination),
            info => new FileDiff(info.FullName, FileDiffStatus.Deleted),
            info => new FileDiff(info.FullName, FileDiffStatus.Created),
            (info, fileInfo) => fileComparer.AreEqual(info, fileInfo)
                ? new FileDiff(info.FullName, FileDiffStatus.Unchanged)
                : new FileDiff(info.FullName, FileDiffStatus.Modified));
    }

    private static IEnumerable<IFileInfo> GetFilesRecursively(IDirectoryInfo origin)
    {
        return MoreEnumerable.TraverseBreadthFirst(origin, dir => dir.EnumerateDirectories())
            .SelectMany(r => r.GetFiles());
    }

    private FileDiff GenerateResult(IFileInfo fileInfo, IFileInfo fileInfo1)
    {
        return new FileDiff("", FileDiffStatus.Invalid);
    }
}

public class FileDiff
{
    public FileDiff(string source, FileDiffStatus status)
    {
        Source = source;
        Status = status;
    }

    public string Source { get; }
    public FileDiffStatus Status { get; }

    public override string ToString()
    {
        return $"{nameof(Source)}: {Source}, {nameof(Status)}: {Status}";
    }
}