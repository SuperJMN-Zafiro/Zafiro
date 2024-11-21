using CSharpFunctionalExtensions;
using FluentAssertions;
using Zafiro.Deployment.Core;
using Zafiro.FileSystem.Mutable;
using File = Zafiro.FileSystem.Local.File;

namespace Zafiro.Deployment.Tests;

public class PublishTests
{
    [Fact]
    public async Task PushToNuGet()
    {
        var fs = new System.IO.Abstractions.FileSystem();
        var fi = fs.FileInfo.New("/home/jmn/package.nupkg/Zafiro.Avalonia.1.0.0.nupkg");
        var package = new File(fi);
        
        var result = await package.AsReadOnly().Bind(file => Publisher.Instance.ToNuGet(file, "ble"));

        result.Should().Succeed();
    }
}

public class DeploymentTests
{
    [Fact]
    public async Task PushSite()
    {
        var projectPath = "F:\\Repos\\SuperJMN-Zafiro\\Zafiro.Avalonia\\samples\\TestApp\\TestApp.Browser\\TestApp.Browser.csproj";
        
        var result = await Deployer.Instance.PublishAvaloniaAppToGitHubPages(projectPath, "SuperJMN-Zafiro", "SuperJMN-Zafiro.github.io", "asdf");
        
        result.Should().Succeed();
    }
}