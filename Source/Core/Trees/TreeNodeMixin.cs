using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Trees;

public static class TreeNodeMixin
{
    public static IEnumerable<T> Flatten<T>(this IEnumerable<T> nodes, Func<T, IEnumerable<T>> getChildren)
    {
        return nodes.SelectMany(node => Flatten(node, getChildren));
    }

    public static IEnumerable<T> Flatten<T>(this T node, Func<T, IEnumerable<T>> getChildren)
    {
        return new[] { node }.Concat(getChildren(node).SelectMany(x => Flatten(x, getChildren)));
    }

    public static IEnumerable<int> GetPath<T>(this T root, T toLocate, Func<T, IEnumerable<T>> getChildren)
    {
        return GetPath(new[] { root }, toLocate, getChildren);
    }

    public static IEnumerable<int> GetPath<T>(this IEnumerable<T> enumerable, T toLocate, Func<T, IEnumerable<T>> getChildren)
    {
        var tree = TreeNode<T>.Create(enumerable, null, getChildren);
        var flatten = Flatten(tree, x => x.Children);
        var node = flatten.FirstOrDefault(x => x.Item.Equals(toLocate));
        return GetPath(node);
    }

    public static IEnumerable<int> GetPath<T>(this TreeNode<T> node)
    {
        if (node.Parent != null)
        {
            foreach (var parentPath in GetPath(node.Parent))
            {
                yield return parentPath;
            }
        }

        yield return node.Index;
    }
}