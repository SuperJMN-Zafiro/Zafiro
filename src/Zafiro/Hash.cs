using System.Linq;

namespace Zafiro.Core;

public class Hash
{
    public byte[] Bytes { get; }

    public Hash(byte[] bytes)
    {
        Bytes = bytes;
    }

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

        if (obj.GetType() != this.GetType())
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