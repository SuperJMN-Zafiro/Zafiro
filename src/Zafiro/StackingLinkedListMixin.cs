using System.Collections.Generic;
using System.Linq;

namespace Zafiro.Zafiro
{
    public static class StackingLinkedListMixin
    {
        public static IEnumerable<T> GetAncestors<T>(this StackingLinkedList<T> sll) where T : class
        {
            return sll.ToList().Reverse();
        }
    }
}