using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace Core;

public class Host : ValueObject
{
    public Host(CaseInsensitiveString identifier)
    {
        Identifier = identifier;
    }

    public CaseInsensitiveString Identifier { get; }


    public static implicit operator string(Host username)
    {
        return username.Identifier;
    }

    public static implicit operator Host(string username)
    {
        return new Host(username);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Identifier;
    }

    public override string ToString()
    {
        return Identifier;
    }
}