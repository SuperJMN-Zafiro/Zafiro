namespace Zafiro.DivineBytes.Unix;

[Flags]
public enum Permission
{
    None = 0,
    OwnerRead = 1 << 0,   // 1
    OwnerWrite = 1 << 1,  // 2
    OwnerExec = 1 << 2,   // 4
    GroupRead = 1 << 3,   // 8
    GroupWrite = 1 << 4,  // 16
    GroupExec = 1 << 5,   // 32
    OtherRead = 1 << 6,   // 64
    OtherWrite = 1 << 7,  // 128
    OtherExec = 1 << 8,   // 256

    // Combinaciones comunes
    OwnerAll = OwnerRead | OwnerWrite | OwnerExec,
    GroupAll = GroupRead | GroupWrite | GroupExec,
    OtherAll = OtherRead | OtherWrite | OtherExec,
    AllRead = OwnerRead | GroupRead | OtherRead,
    AllWrite = OwnerWrite | GroupWrite | OtherWrite,
    AllExec = OwnerExec | GroupExec | OtherExec,
    All = OwnerAll | GroupAll | OtherAll
}

public struct UnixPermissions
{
    public bool OwnerRead { get; }
    public bool OwnerWrite { get; }
    public bool OwnerExec { get; }
    public bool GroupRead { get; }
    public bool GroupWrite { get; }
    public bool GroupExec { get; }
    public bool OtherRead { get; }
    public bool OtherWrite { get; }
    public bool OtherExec { get; }

    public UnixPermissions(bool ownerRead, bool ownerWrite, bool ownerExec,
        bool groupRead, bool groupWrite, bool groupExec,
        bool otherRead, bool otherWrite, bool otherExec)
    {
        OwnerRead = ownerRead;
        OwnerWrite = ownerWrite;
        OwnerExec = ownerExec;
        GroupRead = groupRead;
        GroupWrite = groupWrite;
        GroupExec = groupExec;
        OtherRead = otherRead;
        OtherWrite = otherWrite;
        OtherExec = otherExec;
    }

    // Constructor más cómodo usando flags
    public UnixPermissions(Permission permissions)
    {
        OwnerRead = permissions.HasFlag(Permission.OwnerRead);
        OwnerWrite = permissions.HasFlag(Permission.OwnerWrite);
        OwnerExec = permissions.HasFlag(Permission.OwnerExec);
        GroupRead = permissions.HasFlag(Permission.GroupRead);
        GroupWrite = permissions.HasFlag(Permission.GroupWrite);
        GroupExec = permissions.HasFlag(Permission.GroupExec);
        OtherRead = permissions.HasFlag(Permission.OtherRead);
        OtherWrite = permissions.HasFlag(Permission.OtherWrite);
        OtherExec = permissions.HasFlag(Permission.OtherExec);
    }

    // Método para obtener los permisos como flags
    public Permission ToPermission()
    {
        Permission result = Permission.None;

        if (OwnerRead) result |= Permission.OwnerRead;
        if (OwnerWrite) result |= Permission.OwnerWrite;
        if (OwnerExec) result |= Permission.OwnerExec;
        if (GroupRead) result |= Permission.GroupRead;
        if (GroupWrite) result |= Permission.GroupWrite;
        if (GroupExec) result |= Permission.GroupExec;
        if (OtherRead) result |= Permission.OtherRead;
        if (OtherWrite) result |= Permission.OtherWrite;
        if (OtherExec) result |= Permission.OtherExec;

        return result;
    }

    // Operador de conversión implícita desde Permission
    public static implicit operator UnixPermissions(Permission permissions)
    {
        return new UnixPermissions(permissions);
    }

    // Operador de conversión implícita hacia Permission
    public static implicit operator Permission(UnixPermissions permissions)
    {
        return permissions.ToPermission();
    }
}