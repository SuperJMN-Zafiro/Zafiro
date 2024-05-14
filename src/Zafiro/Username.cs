using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace Zafiro;

public class Username : ValueObject
{
    public Username(CaseInsensitiveString identifier)
    {
        Identifier = identifier;
    }

    public CaseInsensitiveString Identifier { get; }


    public static implicit operator string(Username username)
    {
        return username.Identifier;
    }

    public static implicit operator Username(string username)
    {
        return new Username(username);
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Identifier;
    }
}