using System.IO.Abstractions;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Deployment.Core;
using Zafiro.FileSystem.Core;

namespace Zafiro.Deployment.Services.GitHub;

public class AvaloniaSitePublicationWithGit(AvaloniaSite avaloniaSite, string repositoryOwner, string repositoryName, string apiKey, Maybe<ILogger> logger, string branchName = "master")
{
    public string RepositoryOwner { get; } = repositoryOwner;
    public string RepositoryName { get; } = repositoryName;
    public string BranchName { get; } = branchName;
    public string ApiKey { get; } = apiKey;
    
    private System.IO.Abstractions.FileSystem fileSystem = new();

    public Task<Result> Publish()
    {
        return CreateRepository()
            .Bind(repoDir => AddFilesToRepository(repoDir, avaloniaSite.Files))
            .Bind(repoDir => CommitAndPushChanges(repoDir));
    }

    private Task<Result<IDirectoryInfo>> CreateRepository()
    {
        return Result.Try(() => fileSystem.Directory.CreateTempSubdirectory($"{RepositoryOwner}_{RepositoryName}"))
            .Bind(ConfigureRemoteUrl)
            .Bind(repoDir =>
            {
                return Command.Execute("git", $"pull origin {BranchName}", repoDir.FullName, logger).Map(() => repoDir);
            });
    }

    private async Task<Result<IDirectoryInfo>> AddFilesToRepository(IDirectoryInfo repoDir, IEnumerable<IRootedFile> files)
    {
        foreach (var file in files)
        {
            var targetPath = Path.Combine(repoDir.FullName, file.FullPath().ToString());

            Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
            await file.DumpTo(targetPath);
        }

        // Añade los cambios al índice
        var execute = await Command.Execute("git", "add .", repoDir.FullName, logger);
        return execute.Map(() => repoDir);
    }

    private async Task<Result> CommitAndPushChanges(IDirectoryInfo repoDir)
    {
        // Crea un commit
        var commitResult = await Command.Execute("git", $"commit -m \"Site update: {DateTime.UtcNow}\"", repoDir.FullName, logger);

        // Si no hay cambios, ignora el push
        if (commitResult.IsFailure && commitResult.Error.Contains("nothing to commit"))
        {
            return Result.Success();
        }

        // Realiza el push
        return await Command.Execute("git", $"push origin {BranchName}", repoDir.FullName, logger);
    }

    private Task<Result<IDirectoryInfo>> ConfigureRemoteUrl(IDirectoryInfo repoDir)
    {
        // Configura la URL remota usando el API Key
        var remoteUrl = $"https://{ApiKey}@github.com/{RepositoryOwner}/{RepositoryName}.git";

        return Command.Execute("git", $"remote set-url origin {remoteUrl}", repoDir.FullName, logger).Map(() => repoDir);
    }
}
