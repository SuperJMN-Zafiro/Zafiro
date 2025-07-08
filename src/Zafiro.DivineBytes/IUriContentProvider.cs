using CSharpFunctionalExtensions;

namespace Zafiro.DivineBytes;

public interface IUriContentProvider
{
    Task<Result<IByteSource>> GetByteSourceAsync(Uri uri, CancellationToken cancellationToken = default);
}
