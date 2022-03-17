using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSharpFunctionalExtensions;

namespace Reflection
{
    public class CollectionType
    {
        private static MethodInfo addMethod;

        private static Type childType;

        private readonly Type collectionType;

        private CollectionType(Type collectionType)
        {
            this.collectionType = collectionType;

            InstanceType = new Lazy<Type>(() =>
            {
                if (collectionType.IsInterface)
                {
                    return GetNonInterfaceType();
                }

                return collectionType;
            });
        }

        private Type GetNonInterfaceType()
        {
            var genericEnumerable = typeof(IEnumerable<>).MakeGenericType(childType);
            var genericCollection = typeof(ICollection<>).MakeGenericType(childType);
            var genericList = typeof(IList<>).MakeGenericType(childType);

            var collectionTypes = new[] {genericEnumerable, genericCollection, genericList};
            if (collectionTypes.Contains(collectionType.GetICollectionType()))
            {
                return typeof(List<>).MakeGenericType(childType);
            }

            throw new InvalidOperationException("Not supported collection");
        }

        private Lazy<Type> InstanceType { get; }
        public Type ChildType => childType;

        public object CreateInstance()
        {
            return Activator.CreateInstance(InstanceType.Value);
        }

        public Result Add(object instance, object item)
        {
            if (!item.GetType().IsAssignableTo(childType))
            {
                return Result.Failure(
                    $"Invalid child. The item should be of type {childType}");
            }

            addMethod.Invoke(instance, new[] {item});
            return Result.Success();
        }

        public static Result<CollectionType> Create(Type type)
        {
            if (!type.IsCollection())
            {
                return Result.Failure<CollectionType>($"{type} isn't a collection");
            }

            addMethod = type.GetMethod("Add");
            if (addMethod is null)
            {
                return Result.Failure<CollectionType>("Cannot get the 'Add' method of the collection");
            }

            if (addMethod.GetParameters().Length != 1)
            {
                return Result.Failure<CollectionType>("'Add' method should only contain one parameter");
            }

            childType = addMethod.GetParameters()[0].ParameterType;
            if (childType.IsAssignableTo(type.GetICollectionType()))
            {
                return Result.Failure<CollectionType>(
                    $"The instance of the 'Add' method should be assignable to {type.GetICollectionType()}");
            }

            return new CollectionType(type);
        }
    }
}