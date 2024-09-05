using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

[PublicAPI]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Fid
{
    public int volume_id { get; set; }
    public int file_key { get; set; }
    public long cookie { get; set; }
}