using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Zafiro.Mapping
{
    public abstract class MemberMap
    {
        public MemberInfo Member { get; }

        public MemberMap(MemberInfo member)
        {
            Member = member;
        }

        protected readonly HashSet<string> SourcesCore = new();
        protected Delegate MapCore;

        public Delegate Map => MapCore;

        public object Call(Func<string, object> map)
        {
            return Map.DynamicInvoke(Sources.Select(map).ToArray());
        }

        public IReadOnlyList<string> Sources => SourcesCore.ToImmutableList();
    }
}