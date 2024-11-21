using System.Diagnostics;
using CSharpFunctionalExtensions;
using Zafiro.Deployment;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Readonly;

namespace Zafiro.Nuke;

public class Dotnet : IDotnet
{
    private readonly System.IO.Abstractions.FileSystem filesystem;

    public Dotnet()
    {
        filesystem = new System.IO.Abstractions.FileSystem();
    }

    public Task<Result<IDirectory>> Publish(string projectPath, string arguments = "")
    {
        return Result.Try(() => filesystem.Directory.CreateTempSubdirectory())
            .Map(output =>
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = string.Join(" ", $"publish {projectPath} --output {output.FullName}", arguments),
                    WorkingDirectory = output.FullName,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                };

                return new
                {
                    Process = new Process
                    {
                        StartInfo = processStartInfo,
                    },
                    Output = output
                };

            }).Bind(async publish =>
            {
                publish.Process.Start();
                await publish.Process.WaitForExitAsync();
                var stdError = await publish.Process.StandardError.ReadToEndAsync();
                var stdOutput = await publish.Process.StandardOutput.ReadToEndAsync();
                return Result.FailureIf(publish.Process.ExitCode != 0, $"Could not publish the project: {stdError}:{stdOutput}")
                    .Map(() => publish);
            })
            .Map(tuple => new FileSystem.Local.Directory(tuple.Output))
            .Bind(directory => directory.ToDirectory());
    }
}