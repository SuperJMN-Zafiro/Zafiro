using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using CSharpFunctionalExtensions;

namespace Zafiro.Reflection
{
    public class ReflectedCollection
    {
        private readonly object instance;
        private readonly MemberInfo member;
        private readonly CollectionType collectionType;

        private ReflectedCollection(object instance, MemberInfo member, CollectionType collectionType)
        {
            this.instance = instance;
            this.member = member;
            this.collectionType = collectionType;
        }

        public IEnumerable Items => (IEnumerable) member.GetMemberValue(instance);

        public void Add(object item)
        {
            if (!IsInitialized)
            {
                Initialize();
            }

            collectionType.Add(Items, item);
        }

        private bool IsInitialized => Items != null;
        public Type ItemType => collectionType.ChildType;

        private void Initialize()
        {
            member.SetMemberValue(instance, collectionType.CreateInstance());
        }

        public static Result<ReflectedCollection> From<T>(T instance, Expression<Func<T, object>> collection)
        {
            var member = ReflectionHelper.FindMember(collection);
            return From(instance, member);
        }

        public static Result<ReflectedCollection> From(object instance, MemberInfo member)
        {
            var collectionType = CollectionType.Create(member.GetMemberType());
            if (collectionType.IsFailure)
            {
                return Result.Failure<ReflectedCollection>("Couldn't create the collection type");
            }

            return Result.Success(new ReflectedCollection(instance, member, collectionType.Value));
        }
    }
}