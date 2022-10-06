using Core;
using CSharpFunctionalExtensions;

namespace FileSystem.Caching;

public class CopyOperationMetadata : ValueObject
{
    public CopyOperationMetadata(Host host, ZafiroPath source, ZafiroPath destination, Hash hash)
    {
        Host = host;
        Source = source;
        Destination = destination;
        Hash = hash;
    }

    public Host Host { get; }
    public ZafiroPath Source { get; }
    public ZafiroPath Destination { get; }
    public Hash Hash { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Host;
        yield return Source;
        yield return Destination;
        yield return Hash;
    }
}