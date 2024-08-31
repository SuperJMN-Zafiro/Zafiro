using System.Collections.ObjectModel;

namespace Zafiro;

public class ParentLinkedCollection<TChild, TParent> : Collection<TChild> where TChild : IChild<TParent>
{
    public ParentLinkedCollection(TParent parent)
    {
        Parent = parent;
    }

    public TParent Parent { get; }

    protected override void InsertItem(int index, TChild item)
    {
        item.Parent = Parent;
        base.InsertItem(index, item);
    }
}