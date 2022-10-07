using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Zafiro.Reflection
{
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
        public static MemberInfo FindMember(this LambdaExpression lambdaExpression)
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
}