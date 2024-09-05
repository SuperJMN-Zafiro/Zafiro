using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

[PublicAPI]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class BaseEntry
{
    public string FullPath { get; set; }

    public DateTime Mtime { get; set; }
    public DateTime Crtime { get; set; }
    public ulong Mode { get; set; }
    public ulong Uid { get; set; }
    public ulong Gid { get; set; }
    public string Mime { get; set; }
    public int TtlSec { get; set; }
    public string UserName { get; set; }
    public object GroupNames { get; set; }
    public string SymlinkTarget { get; set; }
    public string? Md5 { get; set; }
    public long FileSize { get; set; }
    public int Rdev { get; set; }
    public ulong Inode { get; set; }
    public object Extended { get; set; }
    public object HardLinkId { get; set; }
    public ulong HardLinkCounter { get; set; }
    public object Content { get; set; }
    public object Remote { get; set; }
    public int Quota { get; set; }
}