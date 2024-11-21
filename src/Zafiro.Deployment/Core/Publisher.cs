using CSharpFunctionalExtensions;
using Zafiro.Deployment.Core;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Readonly;

namespace Zafiro.Deployment;

public class Publisher(IDotnet dotnet)
{
    public Task<Result> ToNuGet(IFile file, string authToken)
    {
        var fs = new System.IO.Abstractions.FileSystem();
        return Result.Try(() => fs.Path.GetTempFileName() + "_" + file.Name)
            .Bind(path => file.DumpTo(path).Map(() => path))
            .Bind(path => dotnet.Push(path, authToken));
    }

    public static Publisher Instance { get; } = new Publisher(new Dotnet());
}