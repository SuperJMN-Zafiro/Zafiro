using System.Diagnostics;
using System.Text;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Mixins;

namespace Zafiro.Deployment.Core;

public static class Command
{
    public static Task<Result> Execute(string command, string arguments, Maybe<string> workingDirectory, Maybe<ILogger> logger)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory.GetValueOrDefault(""),
        };

        return Result.Try(async () =>
            {
                using var process = new Process
                {
                    StartInfo = processStartInfo,
                    EnableRaisingEvents = true,
                };

                logger.Execute(l => l.Information("Executing '{Command}'. Arguments: {Arguments}. Working directory: {WorkingDirectory}", command, arguments, workingDirectory));
                process.Start();

                // Leer salida y error de manera concurrente
                var outputTask = ReadStreamAsync(process.StandardOutput, s => logger.Execute(l => l.Information(s)));
                var errorTask = ReadStreamAsync(process.StandardError, s => logger.Execute(l => l.Information(s)));

                logger.Execute(l => l.Information("Waiting for '{Command}' to execute...", command));
                await process.WaitForExitAsync();

                // Asegurarse de que todas las salidas han sido leídas
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

    private static async Task<string> ReadStreamAsync(StreamReader reader, Action<string> onNewLine)
    {
        var builder = new StringBuilder();
        while (await reader.ReadLineAsync() is { } line)
        {
            builder.AppendLine(line);
            onNewLine(line);
        }
        return builder.ToString();
    }
}
