using System;
using System.Linq;
using CSharpFunctionalExtensions;
using Nuke.Common;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.GitVersion;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Deployment;
using Zafiro.Misc;


[AzurePipelines(AzurePipelinesImage.WindowsLatest, ImportSecrets = new[] { nameof(NuGetApiKey) }, AutoGenerate = false)]
class Build : NukeBuild
{
    [Parameter] [Secret] readonly string NuGetApiKey;
    [Parameter] readonly bool Force;
    [Solution] readonly Solution Solution;
    [GitVersion] readonly GitVersion GitVersion;
    [GitRepository] readonly GitRepository Repository;
   

    Target Publish => td => td
        .Requires(() => NuGetApiKey)
        .OnlyWhenStatic(() => Repository.IsOnMainOrMasterBranch() || Force)
        .Executes(async () =>
        {
            var packableProjects = Solution.AllProjects
                .Where(x => x.GetProperty<bool>("IsPackable"))
                .Where(x => !x.Path.ToString().Contains("Test", StringComparison.InvariantCultureIgnoreCase) && !x.Path.ToString().Contains("Sample", StringComparison.InvariantCultureIgnoreCase)).ToList();
            
            await packableProjects
                .Select(project => Packager.Instance.CreateForNuGet(project, GitVersion.NuGetVersion).LogInfo($"Packing {project}"))
                .CombineSequentially()
                .MapEach(file => Publisher.Instance.ToNuGet(file, NuGetApiKey).LogInfo($"Pushing package {file}"))
                .CombineSequentially()
                .TapError(failed => Assert.Fail(failed));
        });
    
    public static int Main() => Execute<Build>(x => x.Publish);
}