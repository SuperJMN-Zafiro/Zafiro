using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Trees;

public class TreeNode<T>
{
    public static IEnumerable<TreeNode<T>> Create(IEnumerable<T> nodes, TreeNode<T> parent, Func<T, IEnumerable<T>> getChildren)
    {
        return nodes.Select(
            (x, i) =>
            {
                var parentNode = new TreeNode<T>(x, i, parent);
                var children = Create(getChildren(x), parentNode, getChildren);
                parentNode.Children = children;
                return parentNode;
            });
    }

    public TreeNode(T item, int index, TreeNode<T> parent)
    {
        Index = index;
        Item = item;
        Parent = parent;
    }

    public int Index { get; }
    public TreeNode<T>? Parent { get; }
    public T Item { get; }
    public IEnumerable<TreeNode<T>> Children { get; private set; } = null!;
}