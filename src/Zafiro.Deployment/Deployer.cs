using CSharpFunctionalExtensions;
using DotnetPackaging;
using Serilog;
using Zafiro.Deployment.Platforms;
using Zafiro.FileSystem.Readonly;
using Zafiro.Nuke;

namespace Zafiro.Deployment;

public class Deployer
{
    private readonly Maybe<ILogger> logger;
    private readonly Dotnet dotnet;

    public Deployer(Dotnet dotnet, Maybe<ILogger> logger)
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

    public static Deployer Instance { get; set; } = new Deployer(new Dotnet(), Maybe<ILogger>.None);
}