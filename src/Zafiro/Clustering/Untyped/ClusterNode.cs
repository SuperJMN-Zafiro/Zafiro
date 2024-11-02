using System;
using System.Collections.Generic;
using Zafiro.Clustering;

public class ClusterNode
{
    public ClusterNode Parent { get; init; }
    public ClusterNode Left { get; init; }
    public ClusterNode Right { get; init; }
    public double MergeDistance { get; init; }
    public object Content { get; init; }

    public IEnumerable<ClusterNode> Children
    {
        get
        {
            if (Left != null) yield return Left;
            if (Right != null) yield return Right;
        }
    }

    // Constructor para nodos hoja
    private ClusterNode(object content, ClusterNode parent = null)
    {
        Content = content;
        Parent = parent;
        MergeDistance = 0;
        Left = null;
        Right = null;
    }

    // Constructor para nodos internos
    private ClusterNode(ClusterNode left, ClusterNode right, double mergeDistance, ClusterNode parent = null)
    {
        Left = left;
        Right = right;
        MergeDistance = mergeDistance;
        Parent = parent;
        Content = null;

        // Establecer el padre en los nodos hijos si aún no lo tienen
        if (Left != null && Left.Parent == null)
        {
            Left = new ClusterNode(Left.Content, this)
            {
                Left = Left.Left,
                Right = Left.Right,
                MergeDistance = Left.MergeDistance
            };
        }

        if (Right != null && Right.Parent == null)
        {
            Right = new ClusterNode(Right.Content, this)
            {
                Left = Right.Left,
                Right = Right.Right,
                MergeDistance = Right.MergeDistance
            };
        }
    }

    public static ClusterNode Create<T>(ClusterNode<T> node, ClusterNode parent = null)
    {
        if (node is LeafNode<T> leafNode)
        {
            // Crear un nodo hoja del ViewModel
            return new ClusterNode(leafNode.Item, parent);
        }
        else if (node is InternalNode<T> internalNode)
        {
            // Construir recursivamente los nodos hijo
            var leftViewModel = Create(internalNode.Left);
            var rightViewModel = Create(internalNode.Right);

            // Crear un nodo interno del ViewModel
            var internalViewModel = new ClusterNode(leftViewModel, rightViewModel, internalNode.MergeDistance, parent);

            return internalViewModel;
        }
        else
        {
            throw new InvalidOperationException("Tipo de nodo desconocido");
        }
    }
}