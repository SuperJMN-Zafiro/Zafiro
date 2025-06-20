using CSharpFunctionalExtensions;
using Zafiro.DivineBytes;

namespace Zafiro.Deployment.New.Core;

public interface IDotnet
{
    public Task<Result<IDirectory>> Publish(string projectPath, string arguments = "");
    Task<Result> Push(string packagePath, string apiKey);
}