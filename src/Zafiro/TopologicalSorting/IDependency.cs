using System.Collections.Generic;

namespace Zafiro.TopologicalSorting
{
    public interface IDependency<out T>
    {
        IEnumerable<T> Dependencies { get; }
    }
}