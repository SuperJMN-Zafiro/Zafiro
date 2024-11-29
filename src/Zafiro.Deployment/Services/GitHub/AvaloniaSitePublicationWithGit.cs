using System.IO.Abstractions;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Deployment.Core;
using Zafiro.FileSystem.Core;

namespace Zafiro.Deployment.Services.GitHub;

public class AvaloniaSitePublicationWithGit(AvaloniaSite avaloniaSite, string repositoryOwner, string repositoryName, string apiKey, Maybe<ILogger> logger, string authorName, string authorEmail, string branchName = "master")
{
    public string RepositoryOwner { get; } = repositoryOwner;
    public string RepositoryName { get; } = repositoryName;
    public string BranchName { get; } = branchName;
    public string ApiKey { get; } = apiKey;
    public string AuthorName { get; } = authorName;
    public string AuthorEmail { get; } = authorEmail;

    private readonly System.IO.Abstractions.FileSystem fileSystem = new();

    public Task<Result> Publish()
    {
        return CloneRepository()
            .Bind(repoDir => AddFilesToRepository(repoDir, avaloniaSite.Files))
            .Bind(CommitAndPushChanges);
    }

    private Task<Result<IDirectoryInfo>> CloneRepository()
    {
        return Result.Try(() => fileSystem.Directory.CreateTempSubdirectory($"{RepositoryOwner}_{RepositoryName}"))
            .Bind(repoDir =>
            {
                var remoteUrl = $"https://github.com/{RepositoryOwner}/{RepositoryName}.git";
                return Command.Execute("git", $"clone --branch {BranchName} --single-branch --depth 1 {remoteUrl} .", repoDir.FullName, logger)
                    .Map(() => repoDir);
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
        // Configura las variables de entorno para autor y committer
        var environmentVariables = new Dictionary<string, string>
        {
            { "GIT_AUTHOR_NAME", AuthorName },
            { "GIT_AUTHOR_EMAIL", AuthorEmail },
            { "GIT_COMMITTER_NAME", AuthorName },
            { "GIT_COMMITTER_EMAIL", AuthorEmail }
        };

        // Crea un commit
        var commitCommand = $"commit --author=\"{AuthorName} <{AuthorEmail}>\" " +
                            $"-m \"Site update: {DateTime.UtcNow}\"";
        var commitResult = await Command.Execute(
            "git", 
            commitCommand, 
            repoDir.FullName, 
            logger,
            environmentVariables
        );

        // Si no hay cambios, ignora el push
        if (commitResult.IsFailure && commitResult.Error.Contains("nothing to commit"))
        {
            return Result.Success();
        }

        // Realiza el push
        return await Command.Execute(
            "git", 
            $"push https://{ApiKey}@github.com/{RepositoryOwner}/{RepositoryName}.git", 
            repoDir.FullName, 
            logger
        );
    }

}
