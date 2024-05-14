using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace Zafiro;

public class Login : ValueObject
{
    public Login(MachineUser user, string password)
    {
        User = user;
        Password = password;
    }

    public MachineUser User { get; }
    public string Password { get; }
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return User;
        yield return Password;
    }
}