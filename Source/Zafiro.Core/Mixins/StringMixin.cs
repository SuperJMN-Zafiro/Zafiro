using System.Collections.Generic;
using System.IO;

namespace Zafiro.Core.Mixins
{
    public static class StringMixin
    {
        public static IEnumerable<string> Lines(this string s)
        {
            using var tr = new StringReader(s);
            while (tr.ReadLine() is { } l)
            {
                yield return l;
            }
        }
    }
}