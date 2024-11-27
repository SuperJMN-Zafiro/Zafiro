using CSharpFunctionalExtensions;
using Octokit;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Deployment.Core;
using Zafiro.Deployment.Services.GitHub;
using Zafiro.Misc;

namespace Zafiro.Deployment;

public class Deployer(Packager packager, Publisher publisher, Maybe<ILogger> logger)
{
    public Task<Result> PublishPackages(IEnumerable<string> projectToPublish, string version, string nuGetApiKey)
    {
        return projectToPublish
            .Select(project => packager.CreateForNuGet(project, version).LogInfo($"Packing {project}"))
            .CombineSequentially()
            .MapEach(file => publisher.ToNuGet(file, nuGetApiKey).LogInfo($"Pushing package {file}"))
            .CombineSequentially();
    }
    
    public Task<Result> PublishAvaloniaAppToGitHubPages(string projectToPublish, string ownerName, string repositoryName, string apiKey)
    {
        logger.Execute(l => l.Information("Publishing Avalonia WASM application in {Project} to GitHub Pages with owner {Owner}, repository {Repository} ", projectToPublish, ownerName, repositoryName));
        return packager.CreateAvaloniaSite(projectToPublish).LogInfo("Avalonia Site has been packaged")
            .Bind(site => publisher.PublishToGitHubPagesWithGit(site, ownerName, repositoryName, apiKey));
    }

    public static Deployer Instance
    {
        get
        {
            var logger = Maybe<ILogger>.From(Log.Logger);
            var dotnet = new Dotnet(logger);
            var packager = new Packager(dotnet, logger);
            var publisher = new Publisher(dotnet, logger);
            return new(packager, publisher, logger);
        }
    }
}