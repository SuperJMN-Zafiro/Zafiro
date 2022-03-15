using System.Collections.Generic;

namespace Core
{
    public interface IDependency<out T>
    {
        IEnumerable<T> Dependencies { get; }
    }
}