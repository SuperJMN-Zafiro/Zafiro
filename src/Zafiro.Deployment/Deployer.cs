using CSharpFunctionalExtensions;
using Octokit;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Deployment.Core;
using Zafiro.Deployment.Services.GitHub;
using Zafiro.Misc;

namespace Zafiro.Deployment;

public class Deployer
{
    public Task<Result> PublishPackages(IEnumerable<string> projectToPublish, string version, string nuGetApiKey)
    {
        return projectToPublish
            .Select(project => Packager.Instance.CreateForNuGet(project, version).LogInfo($"Packing {project}"))
            .CombineSequentially()
            .MapEach(file => Publisher.Instance.ToNuGet(file, nuGetApiKey).LogInfo($"Pushing package {file}"))
            .CombineSequentially();
    }
    
    public Task<Result> PublishAvaloniaAppToGitHubPages(string projectToPublish, string ownerName, string repositoryName, string apiKey)
    {
        return Packager.Instance.CreateAvaloniaSite(projectToPublish)
            .Bind(site => Publisher.Instance.PublishToGitHubPages(site, ownerName, repositoryName, apiKey));
    }

    public static Deployer Instance { get; } = new();
}