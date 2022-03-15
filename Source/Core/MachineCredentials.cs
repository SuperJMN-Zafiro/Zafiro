using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace Core;

public class MachineCredentials : ValueObject
{
    public MachineCredentials(Username username, string password)
    {
        Username = username;
        Password = password;
    }

    public Username Username { get; }
    public string Password { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Username;
        yield return Password;
    }
}