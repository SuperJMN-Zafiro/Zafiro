using Zafiro.DivineBytes.Unix;

namespace Zafiro.DivineBytes.Tests;

public class TestMetadataResolver : IMetadataResolver
{
    public Metadata ResolveDirectory(INamedContainer dir)
        => new(
            new UnixPermissions(
                ownerRead:  true, ownerWrite: true, ownerExec:  true,
                groupRead:  true, groupWrite: false, groupExec:  true,
                otherRead:  true, otherWrite: false, otherExec:  true),
            OwnerId: 0);

    public Metadata ResolveFile(INamedByteSource file)
    {
        if (file.Name.EndsWith(".sh"))
            return new(
                new UnixPermissions(
                    ownerRead:  true, ownerWrite: false, ownerExec:  true,
                    groupRead:  true, groupWrite: false, groupExec:  true,
                    otherRead:  true, otherWrite: false, otherExec:  true),
                OwnerId: 0);

        if (file.Name.EndsWith(".txt"))
            return new(
                new UnixPermissions(
                    ownerRead:  true, ownerWrite: true, ownerExec:  false,
                    groupRead:  true, groupWrite: false, groupExec:  false,
                    otherRead:  true, otherWrite: false, otherExec:  false),
                OwnerId: 0);

        return new(
            new UnixPermissions(
                ownerRead:  true, ownerWrite: false, ownerExec:  false,
                groupRead:  false, groupWrite: false, groupExec:  false,
                otherRead:  false, otherWrite: false, otherExec:  false),
            OwnerId: 0);
    }
}