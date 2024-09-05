namespace Zafiro.FileSystem.Core;

public record FileProperties(bool IsHidden, DateTimeOffset CreationTime, long Length);