using CSharpFunctionalExtensions;
using DotnetPackaging;
using Serilog;
using Zafiro.Deployment.Core;
using Zafiro.Deployment.Platforms;
using Zafiro.FileSystem.Readonly;

namespace Zafiro.Deployment;

public class Packager
{
    private readonly Maybe<ILogger> logger;
    private readonly Dotnet dotnet;

    public Packager(Dotnet dotnet, Maybe<ILogger> logger)
    {
        this.dotnet = dotnet;
        this.logger = logger;
    }

    public Task<Result<IEnumerable<IFile>>> CreateForWindows(string projectPath, WindowsDeployment.DeploymentOptions deploymentOptions)
    {
        return new WindowsDeployment(dotnet, projectPath, deploymentOptions, logger).Create();
    }
    
    public Task<Result<IEnumerable<IFile>>> CreateForLinux(string projectPath, Options deploymentOptions)
    {
        return new LinuxDeployment(dotnet, projectPath, deploymentOptions, logger).Create();
    }
    
    public Task<Result<IEnumerable<IFile>>> CreateForAndroid(string projectPath, AndroidDeployment.DeploymentOptions deploymentOptions)
    {
        return new AndroidDeployment(dotnet, projectPath, deploymentOptions).Create();
    }

    public static Packager Instance { get; set; } = new Packager(new Dotnet(), Maybe<ILogger>.None);

    public Task<Result<IFile>> CreateForNuGet(string projectPath, string version)
    {
        return dotnet.Pack(projectPath, version);
    }
}