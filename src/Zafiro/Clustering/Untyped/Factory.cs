using System;

namespace Zafiro.Clustering.Untyped;

public static class Factory
{
    public static ClusterNodeViewModel CreateViewModel(ClusterNode<object> node, ClusterNodeViewModel parent = null)
    {
        if (node is LeafNode<object> leafNode)
        {
            return new LeafNodeViewModel(leafNode, parent);
        }
        else if (node is InternalNode<object> internalNode)
        {
            return new InternalNodeViewModel(internalNode, parent);
        }
        else
        {
            throw new InvalidOperationException("Unknown node type");
        }
    }
}