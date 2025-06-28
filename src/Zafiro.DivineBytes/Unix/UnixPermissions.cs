namespace Zafiro.DivineBytes.Unix;
public struct UnixPermissions
{
    public bool OwnerRead   { get; }
    public bool OwnerWrite  { get; }
    public bool OwnerExec   { get; }
    public bool GroupRead   { get; }
    public bool GroupWrite  { get; }
    public bool GroupExec   { get; }
    public bool OtherRead   { get; }
    public bool OtherWrite  { get; }
    public bool OtherExec   { get; }

    public UnixPermissions(bool ownerRead, bool ownerWrite, bool ownerExec,
        bool groupRead, bool groupWrite, bool groupExec,
        bool otherRead, bool otherWrite, bool otherExec)
    {
        OwnerRead  = ownerRead;
        OwnerWrite = ownerWrite;
        OwnerExec  = ownerExec;
        GroupRead  = groupRead;
        GroupWrite = groupWrite;
        GroupExec  = groupExec;
        OtherRead  = otherRead;
        OtherWrite = otherWrite;
        OtherExec  = otherExec;
    }
}