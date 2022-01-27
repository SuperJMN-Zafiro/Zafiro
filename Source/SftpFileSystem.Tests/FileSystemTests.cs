using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.WaitStrategies;
using FluentAssertions;
using Xunit;

namespace SftpFileSystem.Tests;

public class FileSystemTests
{
    [Fact]
    public async Task Read_existing_file()
    {
        await using var _ = await CreateSftpServer();
        var sut = CreateSut();
        sut.File.OpenText("/upload/TextFile1.txt")
            .ReadToEnd()
            .Should()
            .NotBeEmpty();
    }

    [Fact]
    public async Task Enumerate_existing_directory()
    {
        await using var _ = await CreateSftpServer();
        var sut = CreateSut();
        sut.Directory.EnumerateFileSystemEntries("/upload")
            .Should()
            .NotBeEmpty();
    }

    private static FileSystem CreateSut()
    {
        return FileSystem.Create("localhost", 22, "tester", "password");
    }

    private static async Task<IAsyncDisposable> CreateSftpServer()
    {
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("atmoz/sftp")
            .WithCommand("tester:password:1000")
            .WithMount("testdata", "/home/tester/upload")
            .WithName("Sftp")
            .WithPortBinding(22)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(22));

        var container = testcontainersBuilder.Build();
        await container.StartAsync();
        return container;
    }
}