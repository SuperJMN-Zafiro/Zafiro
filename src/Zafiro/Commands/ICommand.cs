using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Zafiro.Commands;

public interface ICommand
{
    public Task<Result<string>> Execute(string command,
        string arguments,
        string workingDirectory = "",
        Dictionary<string, string>? environmentVariables = null);
}