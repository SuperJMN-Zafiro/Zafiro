using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

[PublicAPI]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Chunk
{
    public string file_id { get; set; }
    public int size { get; set; }
    public long modified_ts_ns { get; set; }
    public string e_tag { get; set; }
    public Fid fid { get; set; }
    public bool is_compressed { get; set; }
}