using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.DivineBytes;
using Path = Zafiro.DivineBytes.Path;

namespace Zafiro.Deployment.New;

public class SuperPackager(IDotnet dotnet, Maybe<ILogger> logger)
{
    public Task<Result<IEnumerable<INamedByteSourceWithPath>>> CreateForWindows(Path path, WindowsDeployment.DeploymentOptions deploymentOptions)
    {
        return new WindowsDeployment(dotnet, path, deploymentOptions, logger).Create();
    }
}