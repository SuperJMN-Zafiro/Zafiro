using System.Collections.Generic;

namespace Import.Mapper
{
    public interface IMapping
    {
        ICollection<MemberConfiguration> MemberConfigurations { get; }
    }
}