using System.Collections.Generic;
using System.Linq;

namespace Zafiro.Core
{
    using System;

    public static class EnumExtensions
    {
        public static bool TryParse(Type enumType, string value, out object result)
        {
            if (Enum.IsDefined(enumType, value))
            {
                result = Enum.Parse(enumType, value);
                return true;
            }

            result = null;
            return false;
        }

        public static bool IsSubsetOf<T>(this IEnumerable<T> one, IEnumerable<T> another)
        {
            return !one.Except(another).Any();
        }
    }
}