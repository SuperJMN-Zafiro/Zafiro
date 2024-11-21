using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.Readonly;
using Directory = Zafiro.FileSystem.Local.Directory;

namespace Zafiro.Deployment.Core;

public class Dotnet : IDotnet
{
    private readonly System.IO.Abstractions.FileSystem filesystem = new();

    public Task<Result<IDirectory>> Publish(string projectPath, string arguments = "")
    {
        return Result.Try(() => filesystem.Directory.CreateTempSubdirectory())
            .Map(async outputDir =>
            {
                await Command.Execute("dotnet", $"publish {projectPath} --output {outputDir.FullName}", arguments);
                return new Directory(outputDir);
            })
            .Bind(directory => directory.ToDirectory());
    }

    public Task<Result<IFile>> Pack(string projectPath, string version)
    {
        return Result.Try(() => filesystem.Directory.CreateTempSubdirectory())
            .Map(async outputDir =>
            {
                var arguments = ArgumentsParser.Parse([["output", outputDir.FullName]], [["version", version]]);
                await Command.Execute("dotnet", string.Join(" ", "pack", projectPath, arguments), outputDir.FullName);
                return new Directory(outputDir);
            })
            .Bind(directory => directory.Files()
                .Bind(x => x.TryFirst(file => file.Name.EndsWith(".nupkg")).ToResult("Cannot find any NuGet package in the output folder")))
            .Bind(file => file.AsReadOnly());
    }
}