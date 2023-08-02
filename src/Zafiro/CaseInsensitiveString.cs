﻿using System;

namespace Zafiro.Core;

public class CaseInsensitiveString
{
    public CaseInsensitiveString(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Value { get; }

    public static implicit operator CaseInsensitiveString(string str)
    {
        return new CaseInsensitiveString(str);
    }

    public static implicit operator string(CaseInsensitiveString str)
    {
        return str.Value;
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

        return Equals((CaseInsensitiveString) obj);
    }

    public override int GetHashCode()
    {
        return Value.ToLowerInvariant().GetHashCode();
    }

    protected bool Equals(CaseInsensitiveString other)
    {
        return string.Equals(Value, other.Value, StringComparison.InvariantCultureIgnoreCase);
    }

    public override string ToString()
    {
        return Value;
    }
}