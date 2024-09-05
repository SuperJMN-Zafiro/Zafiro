namespace Zafiro.FileSystem.Unix;

public record UnixFileProperties
{
    public required DateTimeOffset LastModification { get; init; }
    public required UnixFileMode FileMode { get; init; }
    public required Maybe<string> OwnerUsername { get; init; }
    public required Maybe<string> GroupName { get; init; }
    public required Maybe<int> OwnerId { get; init; }
    public required Maybe<int> GroupId { get; init; }
    
    public static UnixFileProperties RegularFileProperties() => new()
    {
        FileMode = "644".ToFileMode(),
        GroupId = 1000,
        OwnerId = 1000,
        GroupName = "root",
        OwnerUsername = "root",
        LastModification = DateTimeOffset.Now
    };
    
    public static UnixFileProperties RegularDirectoryProperties() => new()
    {
        FileMode = "755".ToFileMode(),
        GroupId = 1000,
        OwnerId = 1000,
        GroupName = "root",
        OwnerUsername = "root",
        LastModification = DateTimeOffset.Now
    };

    public static UnixFileProperties ExecutableFileProperties() => RegularFileProperties() with { FileMode = "755".ToFileMode() };
}