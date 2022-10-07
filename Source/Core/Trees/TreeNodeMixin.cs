using System;
using System.Collections.Generic;
using System.Linq;
using Zafiro.Core.Mixins;

namespace Zafiro.Core.Trees;

public static class TreeNodeMixin
{
    public static IEnumerable<TreeNode<T, IEnumerable<int>>> ToTreeNodes<T>(this IEnumerable<T> enumerable, Func<T, IEnumerable<T>> getChildren)
    {
        return TreeNode.Create(enumerable, null, getChildren);
    }

    public static IEnumerable<TreeNode<T, IEnumerable<int>>> ToTreeNodes<T>(this T root, Func<T, IEnumerable<T>> getChildren)
    {
        return ToTreeNodes(new[] { root }, getChildren);
    }

    public static TreeNode<T, TPath> Find<T, TPath>(this IEnumerable<TreeNode<T, TPath>> nodes, Func<T, bool> selector)
    {
        return nodes
            .Flatten(x => x.Children)
            .FirstOrDefault(x => selector(x.Item));
    }

    public static TreeNode<T, TPath> Find<T, TPath>(this TreeNode<T, TPath> node, Func<T, bool> selector)
    {
        return Find(new[] { node }, selector);
    }
}