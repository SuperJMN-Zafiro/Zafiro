using System.Collections.Generic;

namespace Zafiro.Mapping
{
    public interface IMap
    {
        IEnumerable<MemberMap> MemberConfigurations { get; }
    }
}