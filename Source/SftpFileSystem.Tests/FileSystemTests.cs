using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.WaitStrategies;
using FluentAssertions;
using Renci.SshNet;
using Xunit;

namespace SftpFileSystem.Tests;

public class FileSystemTests
{
    [Fact]
    public async Task Read_existing_file()
    {
        var container = await new Builder("localhost", "tester", "password", 22)
            .WithFile("upload/TextFile1.txt", "Hello")
            .Build();

        await using (container)
        {
            var sut = CreateSut();
            var contents = await sut.File.OpenText("upload/TextFile1.txt").ReadToEndAsync();

            contents
                .Should()
                .NotBeEmpty();
        }
    }

    [Fact]
    public async Task Enumerate_existing_directory()
    {
        var container = await new Builder("localhost", "tester", "password", 22)
            .WithFile("upload/TextFile1.txt", "Hello")
            .Build();

        await using (container)
        {
            var sut = CreateSut();
            sut.Directory.EnumerateFileSystemEntries("/upload")
                .Should()
                .NotBeEmpty();
        }
    }

    [Fact]
    public async Task Delete_existing_file()
    {
        var container = await new Builder("localhost", "tester", "password", 22)
            .WithFile("upload/TextFile1.txt", "Hello")
            .Build();

        await using (container)
        {
            var sut = CreateSut();
            var file = sut.FileInfo.FromFileName("upload/TextFile1.txt");
            file.Delete();

            AssertClient(client => client.Exists("upload/TextFile1.txt").Should().BeFalse());
        }
    }

    private static void AssertClient(Action<SftpClient> assert)
    {
        using var client = new SftpClient("localhost", 22, "tester", "password");
        client.Connect();
        assert(client);
    }

    private static FileSystem CreateSut()
    {
        return FileSystem.Create("localhost", 22, "tester", "password");
    }

    public static async Task<IAsyncDisposable> CreateSftpServer()
    {
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("atmoz/sftp")
            .WithCommand("tester:password:::upload")
            .WithName("Sftp")
            .WithPortBinding(22)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(22));

        var container = testcontainersBuilder.Build();
        await container.StartAsync();
        return container;
    }
}