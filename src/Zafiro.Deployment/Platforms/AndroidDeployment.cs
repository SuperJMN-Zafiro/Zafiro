using CSharpFunctionalExtensions;
using Zafiro.Deployment.Core;
using Zafiro.FileSystem.Readonly;

namespace Zafiro.Deployment.Platforms;

public class AndroidDeployment(IDotnet dotnet, string projectPath, AndroidDeployment.DeploymentOptions options) : IDeployment
{
    public Task<Result<IEnumerable<IFile>>> Create()
    {
        var args = CreateArgs(options);
        return dotnet.Publish(projectPath, args)
            .Map(directory => directory.AllFiles()
                .Where(file => file.Name.EndsWith(".apk")));
    }

    private static string CreateArgs(DeploymentOptions deploymentOptions)
    {
        var properties = new[]
        {
            new[] { "ApplicationVersion", deploymentOptions.ApplicationVersion },
            new[] { "ApplicationDisplayVersion", deploymentOptions.ApplicationDisplayVersion },
            new[] { "AndroidKeyStore", "true" },
            new[] { "AndroidSigningKeyStore", deploymentOptions.AndroidSigningKeyStoreFile },
            new[] { "AndroidSigningKeyAlias", deploymentOptions.SigningKeyAlias },
            new[] { "AndroidSigningStorePass", deploymentOptions.SigningStorePass },
            new[] { "AndroidSigningKeyPass", deploymentOptions.SigningKeyPass }
        };

        return ArgumentsParser.Parse([["configuration", "Release"]], properties);
    }

    public class DeploymentOptions
    {
        public required string ApplicationVersion { get; init; }
        public required string ApplicationDisplayVersion { get; init; }
        public required string AndroidSigningKeyStoreFile { get; init; }
        public required string SigningKeyAlias { get; init; }
        public required string SigningStorePass { get; init; }
        public required string SigningKeyPass { get; init; }
    }
}