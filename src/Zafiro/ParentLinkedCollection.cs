using System.Collections.ObjectModel;

namespace Zafiro.Core
{
    public class ParentLinkedCollection<TChild, TParent> : Collection<TChild> where TChild : IChild<TParent>
    {
        public TParent Parent { get; }

        public ParentLinkedCollection(TParent parent)
        {
            Parent = parent;
        }

        protected override void InsertItem(int index, TChild item)
        {
            item.Parent = Parent;
            base.InsertItem(index, item);
        }
    }
}