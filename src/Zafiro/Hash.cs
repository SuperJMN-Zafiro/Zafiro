using System.Linq;

namespace Zafiro;

public class Hash
{
    public Hash(byte[] bytes)
    {
        Bytes = bytes;
    }

    public byte[] Bytes { get; }

    protected bool Equals(Hash other)
    {
        return Bytes.SequenceEqual(other.Bytes);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((Hash)obj);
    }

    public override int GetHashCode()
    {
        return 0;
    }
}