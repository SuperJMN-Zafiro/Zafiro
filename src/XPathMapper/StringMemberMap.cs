using System;
using System.Reflection;

namespace Zafiro.Mapping
{
    public class StringMemberMap : MemberMap
    {
        public StringMemberMap(MemberInfo member) : base(member)
        {
        }

        public StringMemberMap MapFrom(string xpath)
        {
            SourcesCore.Add(xpath);
            MapCore = new Func<string, string>(s => s);
            return this;
        }
    }
}