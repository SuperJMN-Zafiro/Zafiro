using System.Collections.Generic;
using System.Linq;

namespace Zafiro.Uwp.Controls.ObjEditor
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> GetCommon<T>(this IList<IEnumerable<T>> lists)
        {
            return lists.GetCommon(EqualityComparer<T>.Default);
        }

        public static IEnumerable<T> GetCommon<T>(this IList<IEnumerable<T>> lists, IEqualityComparer<T> comparer)
        {
            return lists
                .Skip(1)
                .Aggregate(
                    new HashSet<T>(lists.First(), comparer),
                    (h, e) =>
                    {
                        h.IntersectWith(e);
                        return h;
                    }
                );
        }
    }
}