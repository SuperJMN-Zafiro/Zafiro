using System.Diagnostics;
using System.Text;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Mixins;

namespace Zafiro.Deployment.Core;

public static class Command
{
    public static async Task<Result> Execute(string command, string arguments, Maybe<string> workingDirectory, Maybe<ILogger> logger)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        return await Result.Try(async () =>
            {
                var process = new Process
                {
                    StartInfo = processStartInfo,
                };

                logger.Execute(l => l.Information("Executing '{Command}' with arguments {Arguments}", command, arguments));
                process.Start();

                // Leer salida y error de manera concurrente
                var outputTask = ReadStreamAsync(process.StandardOutput);
                var errorTask = ReadStreamAsync(process.StandardError);

                logger.Execute(l => l.Information("Waiting for '{Command}' to exit", command));
                await process.WaitForExitAsync();

                var output = await outputTask;
                var error = await errorTask;

                return (process.ExitCode, output, error);
            })
            .Bind(result =>
            {
                var (exitCode, output, error) = result;
                if (exitCode != 0)
                {
                    return Result.Failure(new[]
                    {
                        $"Execution of {command} failed.",
                        $"Error: {error}",
                        $"Output: {output}",
                        $"Arguments: {arguments}"
                    }.JoinWithLines());
                }

                return Result.Success();
            });
    }

    private static async Task<string> ReadStreamAsync(StreamReader reader)
    {
        var builder = new StringBuilder();
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line != null)
            {
                builder.AppendLine(line);
            }
        }
        return builder.ToString();
    }
}
