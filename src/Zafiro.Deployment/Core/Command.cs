using System.Diagnostics;
using CSharpFunctionalExtensions;
using Zafiro.Mixins;

namespace Zafiro.Deployment.Core;

public static class Command
{
    public static Task<Result> Execute(string command, string arguments)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
        };
        
        return Result.Try(async () =>
            {
                var process = new Process
                {
                    StartInfo = processStartInfo,
                };

                process.Start();
                await process.WaitForExitAsync();
                return process;
            })
            .Bind(async process =>
            {
                if (process.ExitCode != 0)
                {
                    var stdError = await process.StandardError.ReadToEndAsync();
                    var stdOutput = await process.StandardOutput.ReadToEndAsync();
                    return Result.Failure(new[] { $"Execution of {command} failed.", stdError, stdOutput, $"Arguments: {arguments}" }.JoinWithLines());
                }

                return Result.Success();
            });
    }
}