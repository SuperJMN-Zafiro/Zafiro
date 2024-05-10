using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace Zafiro;

public class MachineCredentials : ValueObject
{
    public MachineCredentials(Username username, string password)
    {
        Username = username;
        Password = password;
    }

    public Username Username { get; }
    public string Password { get; }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Username;
        yield return Password;
    }
}