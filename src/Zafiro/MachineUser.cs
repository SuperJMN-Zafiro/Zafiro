using System;

namespace Zafiro;

public record MachineUser(Host Host, Username Username) : IComparable
{
    public int CompareTo(object? obj)
    {
        if (obj == null)
        {
            return 1;
        }

        if (obj is MachineUser otherMachineUser)
        {
            var hostComparison = StringComparer.InvariantCultureIgnoreCase.Compare(Host, otherMachineUser.Host);
            if (hostComparison != 0)
            {
                return hostComparison;
            }

            return StringComparer.InvariantCultureIgnoreCase.Compare(Username, otherMachineUser.Username);
        }

        throw new ArgumentException("Object is not a MachineUser");
    }
}