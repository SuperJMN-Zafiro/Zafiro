using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Readonly;

namespace Zafiro.Deployment;

public interface IDotnet
{
    public Task<Result<IDirectory>> Publish(string projectPath, string arguments = "");
}