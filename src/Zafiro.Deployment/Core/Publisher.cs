using CSharpFunctionalExtensions;
using Octokit;
using Serilog;
using Zafiro.Deployment.Services.GitHub;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Readonly;

namespace Zafiro.Deployment.Core;

public class Publisher(IDotnet dotnet, Maybe<ILogger> logger)
{
    public Task<Result>  ToNuGet(IFile file, string authToken)
    {
        var fs = new System.IO.Abstractions.FileSystem();
        return Result.Try(() => fs.Path.GetTempFileName() + "_" + file.Name)
            .Bind(path => file.DumpTo(path).Map(() => path))
            .Bind(path => dotnet.Push(path, authToken));
    }

    public Task<Result> PublishToGitHubPagesWithRestApi(AvaloniaSite site, string ownerName, string repositoryName, string apiKey)
    {
        var gitHubClient = new GitHubClient(new ProductHeaderValue("Zafiro"))
        {
            Credentials = new Credentials(apiKey)
        };
        var pages = new AvaloniaSitePublicationWithRestApi(gitHubClient, site, repositoryName, ownerName);
        
        logger.Execute(x => x.Information("Publishing site to pages"));
        return pages.Publish();
    }
    
    public Task<Result> PublishToGitHubPagesWithGit(AvaloniaSite site, string ownerName, string repositoryName, string apiKey)
    {
        var gitHubClient = new GitHubClient(new ProductHeaderValue("Zafiro"))
        {
            Credentials = new Credentials(apiKey)
        };
        var pages = new AvaloniaSitePublicationWithGit(site, ownerName, repositoryName, apiKey, logger);
        
        logger.Execute(x => x.Information("Publishing site to pages"));
        return pages.Publish();
    }
}