using System.Collections.Generic;

namespace Zafiro;

public interface IDependency<out T>
{
    IEnumerable<T> Dependencies { get; }
}