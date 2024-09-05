using JetBrains.Annotations;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
[PublicAPI]
public class FileMetadata : BaseEntry
{
    public List<Chunk> Chunks { get; set; }
}