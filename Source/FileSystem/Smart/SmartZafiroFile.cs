using System.Security.Cryptography;
using Core;
using CSharpFunctionalExtensions;

namespace FileSystem.Smart;

public class SmartZafiroFile : IZafiroFile
{
    private static readonly SHA1 Hasher = SHA1.Create();
    private readonly SmartZafiroFileSystem fileSystem;
    private readonly IZafiroFile inner;

    public SmartZafiroFile(IZafiroFile inner, SmartZafiroFileSystem fileSystem)
    {
        this.inner = inner;
        this.fileSystem = fileSystem;
    }

    public ZafiroPath Path => inner.Path;

    public async Task<Result> CopyTo(IZafiroFile destination)
    {
        var hash = await GetHash().ConfigureAwait(false);
        if (fileSystem.ContainsHash(this, destination, hash))
        {
            return Result.Success();
        }

        fileSystem.AddHash(this, destination, hash);

        return await inner.CopyTo(destination).ConfigureAwait(false);
    }

    public Stream OpenWrite()
    {
        return inner.OpenWrite();
    }

    public Result Delete()
    {
        return inner.Delete().OnSuccessTry(() => fileSystem.RemoveHash(Path));
    }

    public Stream OpenRead()
    {
        return inner.OpenRead();
    }

    public IZafiroFileSystem FileSystem => fileSystem;

    private async Task<Hash> GetHash()
    {
        using (var stream = OpenRead())
        {
            var hashBytes = await Hasher.ComputeHashAsync(stream).ConfigureAwait(false);
            return new Hash(hashBytes);
        }
    }

    public override string ToString() => inner.ToString();
}