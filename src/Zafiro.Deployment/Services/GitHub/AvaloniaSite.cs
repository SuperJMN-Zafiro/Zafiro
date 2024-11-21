using CSharpFunctionalExtensions;
using Zafiro.DataModel;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Readonly;
using File = Zafiro.FileSystem.Readonly.File;
using IDirectory = Zafiro.FileSystem.Readonly.IDirectory;

namespace Zafiro.Deployment;

public class AvaloniaSite
{
    private AvaloniaSite(IEnumerable<IRootedFile> files)
    {
        Files = files;
    }

    public IEnumerable<IRootedFile> Files { get; }

    public static Result<AvaloniaSite> Create(IDirectory directory)
    {
        return directory.Directories().TryFirst(d => d.Name.Equals("wwwroot", StringComparison.OrdinalIgnoreCase)).ToResult($"Cannot find wwwroot folder in {directory}")
            .Map(wwwroot => wwwroot.RootedFiles())
            .Map(GetFilesToPublish)
            .Map(toPublish => new AvaloniaSite(toPublish));
    }

    private static IEnumerable<IRootedFile> GetFilesToPublish(IEnumerable<IRootedFile> inputs)
    {
        IRootedFile noJekyllFile = new RootedFile(ZafiroPath.Empty, new File(".nojekyll", Data.FromString("No Jekyll")));

        var filesToCopy = inputs
            .Where(ShouldUpload)
            .Append(noJekyllFile);

        return filesToCopy;
    }

    private static bool ShouldUpload(IRootedFile x)
    {
        var excludedExtensions = new[] { ".br", ".gz", ".js.map" };

        var hasExcludedExt = excludedExtensions.Any(s => x.FullPath().ToString().EndsWith(s, StringComparison.OrdinalIgnoreCase));
        var shouldUpload = !hasExcludedExt;

        return shouldUpload;
    }
}