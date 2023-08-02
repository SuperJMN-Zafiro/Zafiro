﻿using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace Zafiro.Core;

public class Login : ValueObject
{
    public Login(MachineUser user, string password)
    {
        User = user;
        Password = password;
    }

    public MachineUser User { get; }
    public string Password { get; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return User;
        yield return Password;
    }
}