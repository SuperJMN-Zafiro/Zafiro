using System.Diagnostics.CodeAnalysis;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
public class Directory : BaseEntry
{
    // Directory-specific properties can be added here, if available in the JSON
    public List<BaseEntry> Entries { get; set; }
}