using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.Readonly;
using Directory = Zafiro.FileSystem.Local.Directory;

namespace Zafiro.Deployment.Core;

public class Dotnet : IDotnet
{
    private readonly Maybe<ILogger> logger;
    private readonly System.IO.Abstractions.FileSystem filesystem = new();

    public Dotnet(Maybe<ILogger> logger)
    {
        this.logger = logger;
    }
    
    public Task<Result<IDirectory>> Publish(string projectPath, string arguments = "")
    {
        return Result.Try(() => filesystem.Directory.CreateTempSubdirectory())
            .Map(async outputDir =>
            {
                IEnumerable<string[]> options =
                [
                    ["output", outputDir.FullName],
                ];

                var implicitArguments = ArgumentsParser.Parse(options, []);

                var finalArguments = string.Join(" ", "publish", projectPath, arguments, implicitArguments);

                await Command.Execute("dotnet", finalArguments, logger);
                return new Directory(outputDir);
            })
            .Bind(directory => directory.ToDirectory());
    }

    public Task<Result> Push(string packagePath, string apiKey)
    {
        IEnumerable<string[]> options =
            [
                ["source", "https://api.nuget.org/v3/index.json"],
                ["api-key", apiKey],
            ];
        
        return Command.Execute("dotnet", string.Join(" ", "nuget push", packagePath, ArgumentsParser.Parse(options, [])), logger);
    }

    public Task<Result<IFile>> Pack(string projectPath, string version)
    {
        return Result.Try(() => filesystem.Directory.CreateTempSubdirectory())
            .Map(async outputDir =>
            {
                var arguments = ArgumentsParser.Parse([
                    ["output", outputDir.FullName],
                ], [["version", version]]);
                await Command.Execute("dotnet", string.Join(" ", "pack", projectPath, arguments), logger);
                return new Directory(outputDir);
            })
            .Bind(directory => directory.Files()
                .Bind(x => x.TryFirst(file => file.Name.EndsWith(".nupkg")).ToResult("Cannot find any NuGet package in the output folder")))
            .Bind(file => file.AsReadOnly());
    }
}