using System.Collections.Generic;

namespace Zafiro.Core
{
    public interface IDependency<out T>
    {
        IEnumerable<T> Dependencies { get; }
    }
}