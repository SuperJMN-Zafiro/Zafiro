using CSharpFunctionalExtensions;
using DotnetPackaging;
using Serilog;
using Zafiro.Deployment.New.Core;
using Zafiro.Deployment.New.Platforms.Android;
using Zafiro.Deployment.New.Platforms.Linux;
using Zafiro.Deployment.New.Platforms.Windows;
using Zafiro.DivineBytes;
using Path = Zafiro.DivineBytes.Path;

namespace Zafiro.Deployment.New;

public class Packager(IDotnet dotnet, Maybe<ILogger> logger)
{
    public Task<Result<IEnumerable<INamedByteSourceWithPath>>> CreateForWindows(Path path, WindowsDeployment.DeploymentOptions deploymentOptions)
    {
        return new WindowsDeployment(dotnet, path, deploymentOptions, logger).Create();
    }

    public Task<Result<IEnumerable<INamedByteSourceWithPath>>> CreateForAndroid(Path path, AndroidDeployment.DeploymentOptions options)
    {
        return new AndroidDeployment(dotnet, path, options, logger).Create();
    }
    
    public Task<Result<IEnumerable<INamedByteSourceWithPath>>> CreateForLinux(Path path, Options options)
    {
        return new LinuxDeployment(dotnet, path, options, logger).Create();
    }
}