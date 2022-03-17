using System.Collections.Generic;

namespace Mapping
{
    public interface IMap
    {
        IEnumerable<MemberMap> MemberConfigurations { get; }
    }
}