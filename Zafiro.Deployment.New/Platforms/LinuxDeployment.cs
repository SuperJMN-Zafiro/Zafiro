// using System.IO.Abstractions;
// using CSharpFunctionalExtensions;
// using DotnetPackaging;
// using DotnetPackaging.AppImage;
// using Serilog;
// using Zafiro.CSharpFunctionalExtensions;
// using Zafiro.DivineBytes;
// using Zafiro.Mixins;
// using Architecture = System.Runtime.InteropServices.Architecture;
// using File = System.IO.File;
//
// namespace Zafiro.Deployment.New.Platforms;
//
// public class LinuxDeployment(IDotnet dotnet, string projectPath, Options options, Maybe<ILogger> logger)
// {
//     private static readonly Dictionary<Architecture, (string Runtime, string RuntimeLinux)> LinuxArchitecture = new()
//     {
//         [Architecture.X64] = ("linux-x64", "x86_64"),
//         [Architecture.Arm64] = ("linux-arm64", "arm64")
//     };
//
//     public Task<Result<IEnumerable<INamedByteSourceWithPath>>> Create()
//     {
//         IEnumerable<Architecture> supportedArchitectures = [Architecture.Arm64, Architecture.X64];
//
//         return supportedArchitectures
//             .Select(architecture => CreateAppImage(architecture))
//             .CombineInOrder();
//     }
//
//     private Task<Result<INamedByteSourceWithPath>> CreateAppImage(Architecture architecture)
//     {
//         var publishOptions = new[]
//         {
//             new[] { "configuration", "Release" },
//             new[] { "runtime", LinuxArchitecture[architecture].Runtime },
//             new[] { "self-contained", "true" }
//         };
//
//         var arguments = ArgumentsParser.Parse(publishOptions, []);
//
//         var packagePath = options.Name + "-" + options.Version + "-" + LinuxArchitecture[architecture].RuntimeLinux + ".AppImage";
//
//         return dotnet.Publish(projectPath, arguments)
//             .Bind(directory => AppImage
//                 .From()
//                 .Directory(directory)
//                 .Configure(configuration => configuration.From(options))
//                 .Build()
//                 .Bind(appImage => appImage.ToData())
//                 .Map(IFile (data) => new File(packagePath, data)));
//     }
// }