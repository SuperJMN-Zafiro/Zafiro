namespace FileSystem;

public struct CacheKey
{
    public string Origin { get; set; }
    public string Destination { get; set; }
    public string System { get; set; }

    public bool Equals(CacheKey other)
    {
        return Origin == other.Origin && Destination == other.Destination &&
               System == other.System;
    }

    public override bool Equals(object? obj)
    {
        return obj is CacheKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Origin, Destination, System);
    }
}