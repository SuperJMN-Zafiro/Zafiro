using CSharpFunctionalExtensions;
using DotnetPackaging;
using DotnetPackaging.AppImage.Core;
using Serilog;
using Zafiro.Deployment.New.Core;
using Zafiro.Deployment.New.Platforms.Linux.Adapters;
using Zafiro.DivineBytes;
using AppImage = DotnetPackaging.AppImage.AppImage;
using Architecture = System.Runtime.InteropServices.Architecture;
using File = Zafiro.DivineBytes.File;
using Path = Zafiro.DivineBytes.Path;

namespace Zafiro.Deployment.New.Platforms.Linux;

public class LinuxDeployment(IDotnet dotnet, string projectPath, Options options, Maybe<ILogger> logger)
{
    private static readonly Dictionary<Architecture, (string Runtime, string RuntimeLinux)> LinuxArchitecture = new()
    {
        [Architecture.X64] = ("linux-x64", "x86_64"),
        [Architecture.Arm64] = ("linux-arm64", "arm64")
    };

    public Task<Result<IEnumerable<INamedByteSourceWithPath>>> Create()
    {
        IEnumerable<Architecture> supportedArchitectures = [Architecture.Arm64, Architecture.X64];

        return supportedArchitectures
            .Select(architecture => CreateAppImage(architecture))
            .CombineInOrder();
    }

    private Task<Result<INamedByteSourceWithPath>> CreateAppImage(Architecture architecture)
    {
        var publishOptions = new[]
        {
            new[] { "configuration", "Release" },
            new[] { "runtime", LinuxArchitecture[architecture].Runtime },
            new[] { "self-contained", "true" }
        };

        var arguments = ArgumentsParser.Parse(publishOptions, []);

        var appImageFilename = options.Name + "-" + options.Version + "-" + LinuxArchitecture[architecture].RuntimeLinux + ".AppImage";

        return dotnet.Publish(projectPath, arguments)
            .Bind(divineDir => AppImage
                .From()
                .Directory(new DirectoryAdapter(divineDir))
                .Configure(configuration => configuration.From(options))
                .Build()
                .Bind(appImage => AppImageMixin.ToData(appImage))
                .Map(INamedByteSourceWithPath (data) => new NamedByteSourceWithPath(Path.Empty, new File(appImageFilename, new DataAdapter(data)))));
    }
}