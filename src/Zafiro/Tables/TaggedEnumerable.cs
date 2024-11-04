using System.Collections;
using System.Collections.Generic;
using Zafiro.Mixins;

namespace Zafiro.Tables;

public class TaggedEnumerable<T, TTag>(TTag tag, IEnumerable<T> enumerable) : IEnumerable<T> where TTag : notnull where T : notnull
{
    public TTag Tag { get; } = tag;

    public IEnumerator<T> GetEnumerator()
    {
        return enumerable.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)enumerable).GetEnumerator();
    }

    public override string ToString()
    {
        return "{" + Tag + "}" + ": " + this.JoinWithCommas();
    }
}