using System.Diagnostics;
using System.Text;
using CSharpFunctionalExtensions;
using Serilog;

namespace Zafiro.Deployment.Core;

public class Command(Maybe<ILogger> logger) : ICommand
{
    public Maybe<ILogger> Logger { get; } = logger;

    public async Task<Result> Execute(string command,
        string arguments,
        string workingDirectory = "",
        Dictionary<string, string>? environmentVariables = null)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        if (environmentVariables != null)
        {
            foreach (var (key, value) in environmentVariables)
            {
                processStartInfo.Environment[key] = value;
            }
        }

        using var process = new Process { StartInfo = processStartInfo };
        process.Start();

        // Leer salida estándar y error de manera concurrente
        var outputTask = ReadStreamAsync(process.StandardOutput);
        var errorTask = ReadStreamAsync(process.StandardError);

        await Task.WhenAll(outputTask, errorTask);
        await process.WaitForExitAsync();

        var output = outputTask.Result;
        var error = errorTask.Result;

        // Combina salida y error en un solo mensaje
        var combinedOutput = BuildCombinedLogMessage(output, error);

        // Loguear de acuerdo al código de salida
        if (process.ExitCode == 0)
        {
            Logger.Execute(l => l.Information($"Command succeeded:\n{combinedOutput}"));
            return Result.Success();
        }
        else
        {
            Logger.Execute(l => l.Error($"Command failed with exit code {process.ExitCode}:\n{combinedOutput}"));
            return Result.Failure($"Process failed with exit code {process.ExitCode}");
        }
    }

    private static async Task<string> ReadStreamAsync(StreamReader reader)
    {
        var builder = new StringBuilder();
        while (await reader.ReadLineAsync() is { } line)
        {
            builder.AppendLine(line);
        }

        return builder.ToString();
    }

    private static string BuildCombinedLogMessage(string output, string error)
    {
        var builder = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(output))
        {
            builder.AppendLine("Standard Output:");
            builder.AppendLine(output);
        }

        if (!string.IsNullOrWhiteSpace(error))
        {
            builder.AppendLine("Standard Error:");
            builder.AppendLine(error);
        }

        return builder.ToString().TrimEnd();
    }
}