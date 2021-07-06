using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Zafiro.Reflection;

namespace Zafiro.Mapping
{
    public class TypeMap<T> : IMap
    {
        private readonly Collection<MemberMap> memberConfigurations = new ();
        public IEnumerable<MemberMap> MemberConfigurations => memberConfigurations.ToImmutableList();

        public TypeMap<T> ForMember(Expression<Func<T, string>> memberSelector, Action<StringMemberMap> configure)
        {
            var mapping = new StringMemberMap(memberSelector.FindMember());
            configure(mapping);
            memberConfigurations.Add(mapping);
            return this;
        }

        public TypeMap<T> ForMember<TMember>(Expression<Func<T, TMember>> memberSelector, Action<ConversionMemberMap<TMember>> configure)
        {
            var mapping = new ConversionMemberMap<TMember>(memberSelector.FindMember());
            configure(mapping);
            memberConfigurations.Add(mapping);
            return this;
        }

        public TypeMap<T> ForMember<TMember>(Expression<Func<T, IEnumerable<TMember>>> memberSelector, Action<CollectionMemberMap> configure)
        {
            var mapping = new CollectionMemberMap(memberSelector.FindMember());
            configure(mapping);
            memberConfigurations.Add(mapping);
            return this;
        }
    }
}