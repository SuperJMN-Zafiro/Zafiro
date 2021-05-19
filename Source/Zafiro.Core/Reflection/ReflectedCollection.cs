using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Import.Reflection
{
    public class ReflectedCollection
    {
        private readonly object instance;
        private readonly MemberInfo member;
        private readonly MethodInfo addMethod;
        private readonly Type itemType;
        private readonly Type collectionType;

        public ReflectedCollection(object instance, MemberInfo member)
        {
            if (!member.GetMemberType().IsCollection())
            {
                throw new InvalidOperationException($"{member} isn't a collection");
            }

            collectionType = member.GetMemberType().GetICollectionType();
            
            if (collectionType.GenericParametersCount() == 1)
            {
                throw new InvalidOperationException($"{member} isn't a supported collection");
            }

            itemType = collectionType.GenericTypeArguments.First();

            this.instance = instance;
            this.member = member;
            
            addMethod = member.GetMemberType().GetMethod("Add");
        }

        public IEnumerable Items => (IEnumerable) member.GetMemberValue(instance);

        public void Add(object item)
        {
            if (!IsInitialized)
            {
                Initialize();
            }

            if (IsValidItem(item))
            {
                addMethod.Invoke(Items,  new[] {item});
            }
        }

        private bool IsInitialized => Items != null;
        public Type ItemType => itemType;
        private void Initialize()
        {
            var newCollectionType = GetCollectionInstance();
            var newCollection = Activator.CreateInstance(newCollectionType);

            member.SetMemberValue(instance, newCollection);
        }

        private Type GetCollectionInstance()
        {
            var genericEnumerable = typeof(IEnumerable<>).MakeGenericType(itemType);
            var genericCollection = typeof(ICollection<>).MakeGenericType(itemType);
            var genericList = typeof(IList<>).MakeGenericType(itemType);

            var collectionTypes = new[] {genericEnumerable, genericCollection, genericList};
            if (collectionTypes.Contains(collectionType))
            {
                return typeof(List<>).MakeGenericType(itemType);
            }

            throw new NotSupportedException($"The collection type {collectionType} isn't supported ");
        }

        private bool IsValidItem(object item)
        {
            return itemType.IsAssignableFrom(item.GetType());
        }

        public static ReflectedCollection From<T>(T instance, Expression<Func<T, object>> memberSelector)
        {
            var member = ReflectionHelper.FindProperty(memberSelector);
            return new ReflectedCollection(instance, member);
        }

        public static ReflectedCollection From(object instance, MemberInfo member)
        {
            return new(instance, member);
        }
    }
}