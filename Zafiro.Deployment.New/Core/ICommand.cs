using CSharpFunctionalExtensions;

namespace Zafiro.Deployment.New;

public interface ICommand
{
    public Task<Result> Execute(string command,
        string arguments,
        string workingDirectory = "",
        Dictionary<string, string>? environmentVariables = null);
}