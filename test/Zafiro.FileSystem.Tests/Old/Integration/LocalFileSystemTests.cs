using CSharpFunctionalExtensions;
using Serilog;
using Xunit.Abstractions;
using Zafiro.FileSystem.Local;

namespace Zafiro.FileSystem.Tests.Old.Integration;

public class LocalFileSystemTests
{
    private readonly ITestOutputHelper output;

    public LocalFileSystemTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public async Task Root_is_accessible_in_Linux()
    {
        if (OperatingSystem.IsLinux())
        {
            var localFs = new LocalFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None);
            var root = await localFs.GetDirectory(ZafiroPath.Empty);
            root.Should().Succeed().And.Subject.Value.Path.Should().Be(ZafiroPath.Empty);
        }
    }

    [Fact]
    public async Task Root_has_contents()
    {
        if (OperatingSystem.IsLinux())
        {
            var localFs = new LocalFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None);
            var root = await localFs
                .GetDirectory(ZafiroPath.Empty)
                .Bind(x => x.GetDirectories())
                .Tap(dirs => dirs.ToList().ForEach(d => output.WriteLine(d.Path)));

            root.Should().Succeed().And.Subject.Value.Should().NotBeEmpty();
        }
    }

    [Fact]
    public async Task Path_without_root_is_retrieved()
    {
        if (OperatingSystem.IsLinux())
        {
            var localFs = new LocalFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None);
            var root = await localFs
                .GetDirectory("home/jmn");

            root.Should().Succeed().And.Subject.Value.Path.ToString().Should().NotStartWith("/");
        }
    }

    [Fact]
    public async Task Contents_are_retrieved_using_zafiro_path()
    {
        if (OperatingSystem.IsLinux())
        {
            var localFs = new LocalFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None);
            var root = await localFs
                .GetDirectory("home/jmn")
                .Bind(directory => directory.GetFiles())
                .Tap(files => files.ToList().ForEach(d => output.WriteLine(d.Path)));

            root.Should().Succeed().And.Subject.Value.Should().NotBeEmpty();
            root.Value.Should().NotContain(file => file.Path.ToString().StartsWith("/"));
        }
    }
}