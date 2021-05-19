using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Import.Reflection;

namespace Import.Mapper
{
    public class Mapping<T> : IMapping where T : new()
    {
        private readonly ICollection<MemberConfiguration> memberConfigurations = new Collection<MemberConfiguration>();

        public ICollection<MemberConfiguration> MemberConfigurations => memberConfigurations;

        public Mapping<T> ForMember<TMember>(Expression<Func<T, TMember>> func, Action<IMappingExpression> configure)
        {
            var member = ReflectionHelper.FindProperty(func);
            var myMappingExpression = new MappingExpression();
            configure(myMappingExpression);
            MemberConfigurations.Add(new MemberConfiguration(member, myMappingExpression));

            return this;
        }
    }
}