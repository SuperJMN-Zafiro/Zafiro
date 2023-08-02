using System;
using System.Linq.Expressions;
using System.Reflection;
using CSharpFunctionalExtensions;

namespace Zafiro.Reflection
{
    public class ReflectedMember
    {
        private readonly object instance;
        private readonly MemberInfo member;

        private ReflectedMember(object instance, MemberInfo member)
        {
            this.instance = instance;
            this.member = member;
        }

        public bool IsCollection => member.GetMemberType().IsCollection();
        public Type MemberType => member.GetMemberType();

        public static ReflectedMember From<T>(T instance, Expression<Func<T, object>> memberSelector)
        {
            var member = ReflectionHelper.FindMember(memberSelector);
            return new ReflectedMember(instance, member);
        }

        public static ReflectedMember From(object instance, MemberInfo member)
        {
            return new(instance, member);
        }

        public Result<ReflectedCollection> AsCollection()
        {
            return ReflectedCollection.From(instance, member);
        }

        public void Set(object value)
        {
            member.SetMemberValue(instance, value);
        }

        public override string ToString()
        {
            return $"{instance.GetType()}.{member.Name}. {nameof(IsCollection)}: {IsCollection}, {nameof(MemberType)}: {MemberType}";
        }
    }
}