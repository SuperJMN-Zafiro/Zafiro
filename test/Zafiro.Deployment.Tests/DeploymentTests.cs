using FluentAssertions;
using Zafiro.Deployment.Platforms;

namespace Zafiro.Deployment.Tests;

public class DeploymentTests
{
    [Fact]
    public async Task Test1()
    {
        var deployment = await Deployer.Instance.CreateForWindows("/mnt/fast/Repos/SuperJMN-Zafiro/Zafiro.Avalonia/samples/TestApp/TestApp.Desktop/TestApp.Desktop.csproj", new WindowsDeployment.DeploymentOptions
        {
            Version = "1.0.0",
            PackageName = "TestApp"
        });

        deployment.Should().Succeed();
    }
}