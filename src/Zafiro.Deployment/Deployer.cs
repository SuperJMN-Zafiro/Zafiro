using CSharpFunctionalExtensions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Misc;

namespace Zafiro.Deployment;

public class Deployer
{
    public Task<Result> PublishPackages(IEnumerable<string> projectToPublish, string version, string nuGetApiKey)
    {
        return  projectToPublish
            .Select(project => Packager.Instance.CreateForNuGet(project, version).LogInfo($"Packing {project}"))
            .CombineSequentially()
            .MapEach(file => Publisher.Instance.ToNuGet(file, nuGetApiKey).LogInfo($"Pushing package {file}"))
            .CombineSequentially();
    }

    public static Deployer Instance { get; } = new Deployer();
}