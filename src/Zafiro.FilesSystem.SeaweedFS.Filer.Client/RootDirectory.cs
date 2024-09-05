using JetBrains.Annotations;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

[PublicAPI]
public class RootDirectory
{
    public string Path { get; set; }
    public List<BaseEntry>? Entries { get; set; }
    public int Limit { get; set; }
    public string LastFileName { get; set; }
    public bool ShouldDisplayLoadMore { get; set; }
    public bool EmptyFolder { get; set; }
}