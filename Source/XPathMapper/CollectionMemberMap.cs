using System;
using System.Reflection;

namespace Zafiro.Mapping
{
    public class CollectionMemberMap : MemberMap
    {
        public CollectionMemberMap(MemberInfo member) : base(member)
        {
        }

        public CollectionMemberMap MapFrom(string xpath)
        {
            SourcesCore.Add(xpath);
            MapCore = new Func<object, object>(s => s);
            return this;
        }
    }
}