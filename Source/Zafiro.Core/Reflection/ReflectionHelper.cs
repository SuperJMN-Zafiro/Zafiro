using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Import.Reflection
{
    using static Expression;
    public static class ReflectionHelper
    {
        public static Type GetEnumerableElementType(Type type) => TypeExtensions.GetIEnumerableType(type)?.GenericTypeArguments[0] ?? typeof(object);
        
        public static void SetMemberValue(this MemberInfo propertyOrField, object target, object value)
        {
            if (propertyOrField is PropertyInfo property)
            {
                property.SetValue(target, value, null);
                return;
            }
            if (propertyOrField is FieldInfo field)
            {
                field.SetValue(target, value);
                return;
            }
            throw Expected(propertyOrField);
        }
        private static ArgumentOutOfRangeException Expected(MemberInfo propertyOrField) => new ArgumentOutOfRangeException(nameof(propertyOrField), "Expected a property or field, not " + propertyOrField);
        public static object GetMemberValue(this MemberInfo propertyOrField, object target) => propertyOrField switch
        {
            PropertyInfo property => property.GetValue(target, null),
            FieldInfo field => field.GetValue(target),
            _ => throw Expected(propertyOrField)
        };
        public static MemberInfo[] GetMemberPath(Type type, string fullMemberName)
        {
            var memberNames = fullMemberName.Split(new[] {'.'});
            var members = new MemberInfo[memberNames.Length];
            Type previousType = type;
            for(int index = 0; index < memberNames.Length; index++)
            {
                var currentType = GetCurrentType(previousType);
                var memberName = memberNames[index];
                var property = TypeExtensions.GetInheritedProperty(currentType, memberName);
                if (property != null)
                {
                    previousType = property.PropertyType;
                    members[index] = property;
                }
                else if (TypeExtensions.GetInheritedField(currentType, memberName) is FieldInfo field)
                {
                    previousType = field.FieldType;
                    members[index] = field;
                }
                else
                {
                    var method = TypeExtensions.GetInheritedMethod(currentType, memberName);
                    previousType = method.ReturnType;
                    members[index] = method;
                }
            }
            return members;
            static Type GetCurrentType(Type type) => type.IsGenericType && TypeExtensions.IsCollection(type) ? type.GenericTypeArguments[0] : type;
        }
        public static MemberInfo FindProperty(LambdaExpression lambdaExpression)
        {
            Expression expressionToCheck = lambdaExpression.Body;
            while (true)
            {
                switch (expressionToCheck)
                {
                    case MemberExpression { Member: var member, Expression: { NodeType: ExpressionType.Parameter or ExpressionType.Convert } }:
                        return member;
                    case UnaryExpression { Operand: var operand }:
                        expressionToCheck = operand;
                        break;
                    default:
                        throw new ArgumentException(
                            $"Expression '{lambdaExpression}' must resolve to top-level member and not any child object's properties. You can use ForPath, a custom resolver on the child type or the AfterMap option instead.",
                            nameof(lambdaExpression));
                }
            }
        }
        public static Type GetMemberType(this MemberInfo member) => member switch
        {
            PropertyInfo property => property.PropertyType,
            MethodInfo method => method.ReturnType,
            FieldInfo field => field.FieldType,
            null => throw new ArgumentNullException(nameof(member)),
            _ => throw new ArgumentOutOfRangeException(nameof(member))
        };
    }

    public static class TypeExtensions
    {
        public const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        public const BindingFlags StaticFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        public static MethodInfo StaticGenericMethod(this Type type, string methodName, int parametersCount)
        {
            foreach (MethodInfo foundMethod in type.GetMember(methodName, MemberTypes.Method, StaticFlags & ~BindingFlags.NonPublic))
            {
                if (foundMethod.IsGenericMethodDefinition && foundMethod.GetParameters().Length == parametersCount)
                {
                    return foundMethod;
                }
            }
            throw new ArgumentOutOfRangeException(nameof(methodName), $"Cannot find suitable method {type}.{methodName}({parametersCount} parameters).");
        }

        public static void CheckIsDerivedFrom(this Type derivedType, Type baseType)
        {
            if (!baseType.IsAssignableFrom(derivedType) && !derivedType.IsGenericTypeDefinition && !baseType.IsGenericTypeDefinition)
            {
                throw new ArgumentOutOfRangeException(nameof(derivedType), $"{derivedType} is not derived from {baseType}.");
            }
        }

        public static bool IsDynamic(this Type type) => typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type);

        public static IEnumerable<Type> BaseClassesAndInterfaces(this Type type)
        {
            var currentType = type;
            while ((currentType = currentType.BaseType) != null)
            {
                yield return currentType;
            }
            foreach (var interfaceType in type.GetInterfaces())
            {
                yield return interfaceType;
            }
        }

        public static PropertyInfo GetInheritedProperty(this Type type, string name) => type.GetProperty(name, InstanceFlags) ??
            type.BaseClassesAndInterfaces().Select(t => t.GetProperty(name, InstanceFlags)).FirstOrDefault(p => p != null);

        public static FieldInfo GetInheritedField(this Type type, string name) => type.GetField(name, InstanceFlags) ??
            type.BaseClassesAndInterfaces().Select(t => t.GetField(name, InstanceFlags)).FirstOrDefault(f => f != null);

        public static MethodInfo GetInheritedMethod(this Type type, string name) => type.GetMethod(name, InstanceFlags) ??
            type.BaseClassesAndInterfaces().Select(t => t.GetMethod(name, InstanceFlags)).FirstOrDefault(m => m != null)
            ?? throw new ArgumentOutOfRangeException(nameof(name), $"Cannot find member {name} of type {type}.");

        public static MemberInfo GetFieldOrProperty(this Type type, string name)
            => type.GetInheritedProperty(name) ?? (MemberInfo)type.GetInheritedField(name) ?? throw new ArgumentOutOfRangeException(nameof(name), $"Cannot find member {name} of type {type}.");

        public static bool IsNullableType(this Type type) => type.IsGenericType(typeof(Nullable<>));

        public static Type GetICollectionType(this Type type) => type.GetGenericInterface(typeof(ICollection<>));

        public static bool IsCollection(this Type type) => type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);

        public static bool IsListType(this Type type) => typeof(IList).IsAssignableFrom(type);

        public static bool IsGenericType(this Type type, Type genericType) => type.IsGenericType && type.GetGenericTypeDefinition() == genericType;

        public static Type GetIEnumerableType(this Type type) => type.GetGenericInterface(typeof(IEnumerable<>));

        public static Type GetGenericInterface(this Type type, Type genericInterface)
        {
            if (type.IsGenericType(genericInterface))
            {
                return type;
            }
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType(genericInterface))
                {
                    return interfaceType;
                }
            }
            return null;
        }

        public static IEnumerable<ConstructorInfo> GetDeclaredConstructors(this Type type) => type.GetConstructors(InstanceFlags);

        public static int GenericParametersCount(this Type type) => type.GetTypeInfo().GenericTypeParameters.Length;

        public static IEnumerable<Type> GetTypeInheritance(this Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        public static MethodInfo GetStaticMethod(this Type type, string name) => type.GetMethod(name, StaticFlags);
    }
}