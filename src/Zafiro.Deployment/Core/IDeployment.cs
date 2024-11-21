using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Readonly;

namespace Zafiro.Deployment;

public interface IDeployment
{
    Task<Result<IEnumerable<IFile>>> Create();
}