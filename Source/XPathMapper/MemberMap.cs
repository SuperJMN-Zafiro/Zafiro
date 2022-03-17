using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using CSharpFunctionalExtensions;

namespace Mapping
{
    public abstract class MemberMap
    {
        public MemberInfo Member { get; }
        protected Maybe<object> DefaultValue { get; set; } = Maybe<object>.None;

        public MemberMap(MemberInfo member)
        {
            Member = member;
        }

        protected readonly HashSet<string> SourcesCore = new();
        protected Delegate MapCore;

        public Delegate Map => MapCore;

        public object Call(Func<string, Maybe<string>> map)
        {
            var objects = Sources.Select(map).ToArray();

            if (objects.Any(x => x.HasNoValue))
            {
                if (DefaultValue.HasNoValue)
                {
                    throw new InvalidOperationException("");
                }

                return DefaultValue.Value;
            }

            object[] invokeArgs = objects.Select(x => x.Value).ToArray();
            return Map.DynamicInvoke(invokeArgs);
        }

        public IReadOnlyList<string> Sources => SourcesCore.ToImmutableList();
    }
}