using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Deployment.Core;
using Zafiro.Deployment.New;

namespace Zafiro.Deployement.New.Tests.Integration;

public class PackagingTests
{
    [Fact]
    public async Task TestWindows()
    {
        var dotnet = new Dotnet(new Command(Maybe<ILogger>.None), Maybe<ILogger>.None);
        
        var options = new WindowsDeployment.DeploymentOptions
        {
            Version = "1.0.0",
            PackageName = "TestApp"
        };
        
        var result = await new SuperPackager(dotnet, Maybe<ILogger>.None)
            .CreateForWindows("/mnt/fast/Repos/SuperJMN-Zafiro/Zafiro.Avalonia/samples/TestApp/TestApp.Desktop/TestApp.Desktop.csproj", options);
    }
}