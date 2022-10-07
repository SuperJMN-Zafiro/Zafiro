using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.WaitStrategies;
using FluentAssertions;
using Renci.SshNet;
using Xunit;

namespace Zafiro.SftpFileSystem.Tests;

[Trait("Category", "End to end")]
public class FileSystemTests
{
    private const string rootFolder = "upload";
    private static readonly string host = "localhost";
    private static readonly int hostPort = 22;
    private static readonly string username = "tester";
    private static readonly string password = "password";

    public static async Task<IAsyncDisposable> CreateSftpServer()
    {
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("atmoz/sftp")
            .WithCommand($"{username}:{password}:::{rootFolder}")
            .WithName("Sftp")
            .WithPortBinding(hostPort)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(hostPort));

        var container = testcontainersBuilder.Build();
        await container.StartAsync().ConfigureAwait(false);
        return container;
    }

    [Fact]
    public async Task Read_existing_file()
    {
        var container = await CreateBuilder()
            .WithFile("upload/TextFile1.txt", "Hello")
            .Build().ConfigureAwait(false);

        await using (container.ConfigureAwait(false))
        {
            var sut = CreateSut();
            var contents = await sut.File.OpenText($"{rootFolder}/TextFile1.txt").ReadToEndAsync().ConfigureAwait(false);

            contents
                .Should()
                .NotBeEmpty();
        }
    }

    [Fact]
    public async Task Enumerate_existing_directory()
    {
        var container = await CreateBuilder()
            .WithFile($"{rootFolder}/TextFile1.txt", "Hello")
            .Build().ConfigureAwait(false);

        await using (container.ConfigureAwait(false))
        {
            var sut = CreateSut();
            sut.Directory.EnumerateFileSystemEntries($"/{rootFolder}")
                .Should()
                .NotBeEmpty();
        }
    }

    [Fact]
    public async Task Delete_existing_file()
    {
        var container = await CreateBuilder()
            .WithFile($"{rootFolder}/TextFile1.txt", "Hello")
            .Build().ConfigureAwait(false);

        await using (container.ConfigureAwait(false))
        {
            var sut = CreateSut();
            var file = sut.FileInfo.FromFileName("upload/TextFile1.txt");
            file.Delete();

            AssertClient(client => client.Exists($"{rootFolder}/TextFile1.txt").Should().BeFalse());
        }
    }

    [Fact]
    public async Task Delete_folder_non_recursively()
    {
        var container = await CreateBuilder()
            .WithFile($"{rootFolder}/Dir/TextFile1.txt", "Hello")
            .WithFile($"{rootFolder}/Dir/TextFile2.txt", "Hello")
            .Build().ConfigureAwait(false);

        await using (container.ConfigureAwait(false))
        {
            var sut = CreateSut();
            var subDir = sut.DirectoryInfo.FromDirectoryName($"{rootFolder}/Dir");
            subDir.Delete(false);

            AssertClient(client => client.Exists($"{rootFolder}/Dir").Should().BeFalse());
        }
    }

    [Fact]
    public async Task Delete_folder_recursively()
    {
        var container = await CreateBuilder()
            .WithFile($"{rootFolder}/Dir/TextFile1.txt")
            .WithFile($"{rootFolder}/Dir/TextFile2.txt")
            .WithFile($"{rootFolder}/Dir/Subdir/TextFile1.txt")
            .WithFile($"{rootFolder}/Dir/Subdir/TextFile2.txt")
            .Build().ConfigureAwait(false);

        await using (container.ConfigureAwait(false))
        {
            var sut = CreateSut();
            var subDir = sut.DirectoryInfo.FromDirectoryName($"{rootFolder}/Dir");
            subDir.Delete(true);

            AssertClient(client => client.Exists($"{rootFolder}/Dir").Should().BeFalse());
        }
    }

    [Fact]
    public async Task Create_file()
    {
        var container = await CreateBuilder()
            .Build().ConfigureAwait(false);

        await using (container.ConfigureAwait(false))
        {
            var sut = CreateSut();
            var fileInfo = sut.FileInfo.FromFileName($"{rootFolder}/newfile.txt");
            fileInfo.Create();

            AssertClient(client => client.Exists($"{rootFolder}/newfile.txt").Should().BeTrue());
        }
    }

    [Theory]
    [InlineData("/")]
    [InlineData($"{rootFolder}")]
    [InlineData($"{rootFolder}/Dir")]
    [InlineData($"{rootFolder}/Dir/Subdir")]
    [InlineData($"{rootFolder}/Dir/Subdir/Subdir2")]
    public async Task Create_directory(string directoryPath)
    {
        var container = await CreateBuilder()
            .Build().ConfigureAwait(false);

        await using (container.ConfigureAwait(false))
        {
            var sut = CreateSut();
            var subDir = sut.DirectoryInfo.FromDirectoryName(directoryPath);
            subDir.Create();

            AssertClient(client => client.Exists(directoryPath).Should().BeTrue());
        }
    }

    [Fact]
    public async Task Create_subdirectory()
    {
        var container = await CreateBuilder()
            .Build().ConfigureAwait(false);

        await using (container.ConfigureAwait(false))
        {
            var sut = CreateSut();
            var dir = sut.DirectoryInfo.FromDirectoryName($"{rootFolder}");
            dir.CreateSubdirectory("Subdir");

            AssertClient(client => client.Exists($"{rootFolder}/Subdir").Should().BeTrue());
        }
    }

    private static void AssertClient(Action<SftpClient> assert)
    {
        using var client = new SftpClient(host, hostPort, username, password);
        client.Connect();
        assert(client);
    }

    private static FileSystem CreateSut()
    {
        return FileSystem.Connect(host, hostPort, new Credentials(username, password)).Value;
    }

    private static SftpFileSystemBuilder CreateBuilder()
    {
        return new SftpFileSystemBuilder(host, username, password, hostPort);
    }
}