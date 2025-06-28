namespace Zafiro.DivineBytes.Unix;

public interface IPermissioned
{
    UnixPermissions Permissions { get; }
}