using System.Net.Http.Headers;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Octokit;
using Serilog;
using Zafiro.Deployment.Services.GitHub;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Readonly;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace Zafiro.Deployment.Core;

public class Publisher(IDotnet dotnet, Maybe<ILogger> logger, IHttpClientFactory httpClientFactory)
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
        logger.Execute(x => x.Information("Publishing site to pages"));

        return GetGitHubUserInfo(apiKey)
            .Bind(userData =>
            {
                var pages = new AvaloniaSitePublicationWithGit(site, ownerName, repositoryName, apiKey, logger, userData.User, userData.Email);
                return pages.Publish();
            });
    }
    
    private async Task<Result<(string User, string Email)>> GetGitHubUserInfo(string apiKey)
    {
        logger.Execute(x => x.Information("Extracting user info from API Key"));
        
        try
        {
            var httpClient = httpClientFactory.CreateClient("GitHub");
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Zafiro.Deployment", "1.0"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await httpClient.GetAsync("https://api.github.com/user");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;

            var user = root.GetProperty("name").GetString() ?? root.GetProperty("login").GetString() ?? "GitHub User";
            var email = root.GetProperty("email").GetString() ?? $"{root.GetProperty("login").GetString()}@users.noreply.github.com";
            
            logger.Execute(x => x.Information("Got information from API Key using GitHub's API => User: {User}. Email: {Email}", user, email));

            return Result.Success((User: user, Email: email));
        }
        catch (Exception ex)
        {
            return Result.Failure<(string name, string email)>($"Error getting GitHub user info: {ex.Message}");
        }
    }
}