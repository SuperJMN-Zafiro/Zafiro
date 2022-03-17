using System;

namespace Reflection
{
    public static class Extensions
    {
        public static T Get<T>(this object source, string name)
        {
            var type = source.GetType();
            var prop = type.GetProperty(name);

            if (prop is null)
            {
                throw new InvalidOperationException($"Cannot find property {name} on type {type}");
            }

            var isValid = prop.GetValue(source);
            return (T)isValid;
        }
    }
}