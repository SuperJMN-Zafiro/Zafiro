using System.Collections.Generic;

namespace Zafiro.Zafiro
{
    public interface IDependency<out T>
    {
        IEnumerable<T> Dependencies { get; }
    }
}