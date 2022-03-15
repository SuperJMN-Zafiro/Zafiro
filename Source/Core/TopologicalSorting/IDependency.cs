using System.Collections.Generic;

namespace Core.TopologicalSorting
{
    public interface IDependency<out T>
    {
        IEnumerable<T> Dependencies { get; }
    }
}