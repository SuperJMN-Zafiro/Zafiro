using System.Diagnostics;
using CSharpFunctionalExtensions;
using MoreLinq;
using Octokit;
using Zafiro.FileSystem.Core;

namespace Zafiro.Deployment.Services.GitHub;

public class AvaloniaSitePublication(GitHubClient client, AvaloniaSite avaloniaSite, string repositoryName, string repositoryOwner, string branchName = "master")
{
    public string RepositoryOwner { get; } = repositoryOwner;
    public string BranchName { get; } = branchName;
    public string RepositoryName { get; } = repositoryName;
    public GitHubClient Client { get; } = client;

    public Task<Result> Publish()
    {
        var result = GetTree(avaloniaSite.Files)
            .Bind(PushTree);

        return result;
    }

    private Task<Result> PushTree(IEnumerable<NewTreeItem> items)
    {
        return Result.Try(() => Commit(items));
    }

    private async Task Commit(IEnumerable<NewTreeItem> items)
    {
        var reference = await Client.Git.Reference.Get(RepositoryOwner, RepositoryName, $"heads/{BranchName}");
        var latestCommit = await Client.Git.Commit.Get(RepositoryOwner, RepositoryName, reference.Object.Sha);

        var newTree = new NewTree();

        items.ForEach(item => newTree.Tree.Add(item));

        var createdTree = await Client.Git.Tree.Create(RepositoryOwner, RepositoryName, newTree);

        var newCommit = new NewCommit("Site update", createdTree.Sha, latestCommit.Sha);
        var commit = await Client.Git.Commit.Create(RepositoryOwner, RepositoryName, newCommit);

        await Client.Git.Reference.Update(RepositoryOwner, RepositoryName, $"heads/{BranchName}", new ReferenceUpdate(commit.Sha));
    }

    private Task<Result<IEnumerable<NewTreeItem>>> GetTree(IEnumerable<IRootedFile> files)
    {
        return files.Select(file => Result.Try(() => NewTreeItem(file))).CombineInOrder();
    }

    private async Task<NewTreeItem> NewTreeItem(IRootedFile file)
    {
        var base64String = Convert.ToBase64String(file.Bytes());

        var blob = await Client.Git.Blob.Create(RepositoryOwner, RepositoryName, new NewBlob
        {
            Content = base64String,
            Encoding = EncodingType.Base64
        });

        return new NewTreeItem
        {
            Path = file.FullPath(),
            Mode = "100644",
            Type = TreeType.Blob,
            Sha = blob.Sha
        };
    }
}