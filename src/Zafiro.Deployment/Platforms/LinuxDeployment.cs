using CSharpFunctionalExtensions;
using DotnetPackaging;
using DotnetPackaging.AppImage.Core;
using Serilog;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Deployment.Core;
using Zafiro.FileSystem.Readonly;
using AppImage = DotnetPackaging.AppImage.AppImage;
using File = Zafiro.FileSystem.Readonly.File;

namespace Zafiro.Deployment.Platforms;

public class LinuxDeployment(IDotnet dotnet, string projectPath, Options options, Maybe<ILogger> logger) : IDeployment
{
    private static readonly Dictionary<Architecture, (string Runtime, string RuntimeLinux)> LinuxArchitecture = new()
    {
        [Architecture.X64] = ("linux-x64", "x86_64"),
        [Architecture.Arm64] = ("linux-arm64", "arm64")
    };

    public Task<Result<IEnumerable<IFile>>> Create()
    {
        IEnumerable<Architecture> supportedArchitectures = [Architecture.Arm64, Architecture.X64];

        return supportedArchitectures
            .Select(architecture => CreateAppImage(architecture).Tap(() => logger.Tap(l => l.Information("Publishing AppImage for {Architecture}", architecture))))
            .CombineInOrder();
    }

    private Task<Result<IFile>> CreateAppImage(Architecture architecture)
    {
        var publishOptions = new[]
        {
            new[] { "configuration", "Release" },
            new[] { "runtime", LinuxArchitecture[architecture].Runtime },
            new[] { "self-contained", "true" }
        };

        var arguments = ArgumentsParser.Parse(publishOptions, []);

        var packagePath = options.Name + "-" + options.Version + "-" + LinuxArchitecture[architecture].RuntimeLinux + ".AppImage";

        return dotnet.Publish(projectPath, arguments)
            .Bind(directory => AppImage
                .From()
                .Directory(directory)
                .Configure(configuration => configuration.From(options))
                .Build()
                .Bind(appImage => appImage.ToData())
                .Map(IFile (data) => new File(packagePath, data)));
    }
}