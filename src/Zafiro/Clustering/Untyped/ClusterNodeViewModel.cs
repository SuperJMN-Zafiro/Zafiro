using System;

namespace Zafiro.Clustering.Untyped;

public abstract class ClusterNodeViewModel
{
    public ClusterNodeViewModel? Parent { get; }

    protected ClusterNodeViewModel(ClusterNodeViewModel parent)
    {
        Parent = parent;
    }

    public static ClusterNodeViewModel CreateViewModel<T>(ClusterNode<T> node, ClusterNodeViewModel parent = null)
    {
        var objectNode = ConvertToObjectNode(node);

        if (objectNode is LeafNode<object> leafNode)
        {
            return new LeafNodeViewModel(leafNode, parent);
        }
        else if (objectNode is InternalNode<object> internalNode)
        {
            return new InternalNodeViewModel(internalNode, parent);
        }
        else
        {
            throw new InvalidOperationException("Tipo de nodo desconocido");
        }
    }

    public static ClusterNode<object> ConvertToObjectNode<T>(ClusterNode<T> node)
    {
        if (node is LeafNode<T> leafNode)
        {
            return new LeafNode<object>(leafNode.Item); // Convierte el `Item` a `object`
        }
        else if (node is InternalNode<T> internalNode)
        {
            var leftAsObject = ConvertToObjectNode(internalNode.Left);
            var rightAsObject = ConvertToObjectNode(internalNode.Right);
            return new InternalNode<object>(leftAsObject, rightAsObject, internalNode.MergeDistance);
        }

        throw new InvalidOperationException("Tipo de nodo desconocido");
    }
}