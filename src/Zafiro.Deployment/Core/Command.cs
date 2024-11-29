using System.Diagnostics;
using System.Text;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Mixins;

namespace Zafiro.Deployment.Core;

public static class Command
{
    public static async Task<Result> Execute(string command, string arguments, Maybe<string> workingDirectory, Maybe<ILogger> logger, Dictionary<string, string>? environmentVariables = null)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            WorkingDirectory = workingDirectory.GetValueOrDefault(""),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        // Configura las variables de entorno
        if (environmentVariables != null)
        {
            foreach (var (key, value) in environmentVariables)
            {
                processStartInfo.Environment[key] = value;
            }
        }

        using var process = new Process { StartInfo = processStartInfo };
        process.Start();

        // Leer salida y error de manera concurrente
        var output = await ReadStreamAsync(process.StandardOutput, s => logger.Execute(l => l.Information(s)));
        var error = await ReadStreamAsync(process.StandardError, s => logger.Execute(l => l.Information(s)));

        await process.WaitForExitAsync();

        logger.Execute(l => l.Information(output));
        if (!string.IsNullOrWhiteSpace(error))
        {
            logger.Execute(l => l.Error(output));
        }

        return process.ExitCode == 0 ? Result.Success() : Result.Failure(error);
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