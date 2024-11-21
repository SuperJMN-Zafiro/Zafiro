using FluentAssertions;
using Zafiro.Deployment.Platforms;

namespace Zafiro.Deployment.Tests;

public class PackageTests
{
    [Fact]
    public async Task CreateForWindows()
    {
        var deployment = await Packager.Instance.CreateForWindows("/mnt/fast/Repos/SuperJMN-Zafiro/Zafiro.Avalonia/samples/TestApp/TestApp.Desktop/TestApp.Desktop.csproj", new WindowsDeployment.DeploymentOptions
        {
            Version = "1.0.0",
            PackageName = "TestApp"
        });

        deployment.Should().Succeed();
    }
    
    [Fact]
    public async Task Package()
    {
        var deployment = await Packager.Instance.CreateForNuGet("/mnt/fast/Repos/SuperJMN-Zafiro/Zafiro.Avalonia/src/Zafiro.Avalonia/Zafiro.Avalonia.csproj", "1.2.3");

        deployment.Should().Succeed();
    }
}