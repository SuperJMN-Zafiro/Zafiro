﻿using System.Collections.ObjectModel;

namespace Zafiro.Zafiro
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