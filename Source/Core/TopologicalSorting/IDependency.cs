using System.Collections.Generic;

namespace Zafiro.Core.TopologicalSorting
{
    public interface IDependency<out T>
    {
        IEnumerable<T> Dependencies { get; }
    }
}