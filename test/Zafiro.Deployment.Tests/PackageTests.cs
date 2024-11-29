using CSharpFunctionalExtensions;
using FluentAssertions;
using Serilog;
using Zafiro.Deployment.Core;
using Zafiro.Deployment.Platforms;

namespace Zafiro.Deployment.Tests;

public class PackageTests
{
    [Fact]
    public async Task CreateForWindows()
    {
        var deployment = await CreateSut().CreateForWindows("/mnt/fast/Repos/SuperJMN-Zafiro/Zafiro.Avalonia/samples/TestApp/TestApp.Desktop/TestApp.Desktop.csproj", new WindowsDeployment.DeploymentOptions
        {
            Version = "1.0.0",
            PackageName = "TestApp"
        });

        deployment.Should().Succeed();
    }

    [Fact]
    public async Task Package()
    {
        var deployment = await CreateSut().CreateForNuGet("/mnt/fast/Repos/SuperJMN-Zafiro/Zafiro.Avalonia/src/Zafiro.Avalonia/Zafiro.Avalonia.csproj", "1.2.3");

        deployment.Should().Succeed();
    }

    private static Packager CreateSut()
    {
        var logger = new Maybe<ILogger>();
        var command = new Command(logger);
        var dotnet = new Dotnet(command, logger);
        return new Packager(dotnet, Maybe<ILogger>.None);
    }
}