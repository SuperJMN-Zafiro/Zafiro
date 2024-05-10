using System;

namespace Zafiro;

public record MachineUser(Host Host, Username Username) : IComparable
{
    public int CompareTo(object? obj)
    {
        if (obj == null) return 1;
        if (obj is MachineUser otherMachineUser)
        {
            int hostComparison = StringComparer.InvariantCultureIgnoreCase.Compare(this.Host, otherMachineUser.Host);
            if (hostComparison != 0) return hostComparison;
            return StringComparer.InvariantCultureIgnoreCase.Compare(this.Username, otherMachineUser.Username);
        }
        throw new ArgumentException("Object is not a MachineUser");
    }
}