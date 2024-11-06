using System;
using System.Collections.Generic;

namespace Zafiro.DataAnalysis.Clustering;

public class ClusterNode : ICluster
{
    public ClusterNode? Parent { get; init; }
    public ClusterNode? Left { get; init; }
    public ClusterNode? Right { get; init; }

    ICluster? ICluster.Parent => Parent;
    ICluster? ICluster.Left => Left;
    ICluster? ICluster.Right => Right;
    IEnumerable<ICluster> ICluster.Children => Children;

    public double MergeDistance { get; init; }
    public object? Item { get; init; }

    public IEnumerable<ClusterNode> Children
    {
        get
        {
            if (Left != null) yield return Left;
            if (Right != null) yield return Right;
        }
    }

    // Constructor para nodos hoja
    private ClusterNode(object? item, ClusterNode? parent = null)
    {
        Item = item;
        Parent = parent;
        MergeDistance = 0;
        Left = null;
        Right = null;
    }

    // Constructor para nodos internos
    private ClusterNode(ClusterNode left, ClusterNode right, double mergeDistance, ClusterNode? parent = null)
    {
        Left = left;
        Right = right;
        MergeDistance = mergeDistance;
        Parent = parent;
        Item = null;

        // Establecer el padre en los nodos hijos si aún no lo tienen
        if (Left != null && Left.Parent == null)
        {
            Left = new ClusterNode(Left.Item, this)
            {
                Left = Left.Left,
                Right = Left.Right,
                MergeDistance = Left.MergeDistance
            };
        }

        if (Right != null && Right.Parent == null)
        {
            Right = new ClusterNode(Right.Item, this)
            {
                Left = Right.Left,
                Right = Right.Right,
                MergeDistance = Right.MergeDistance
            };
        }
    }

    public static ClusterNode Create<T>(Cluster<T> node, ClusterNode parent = null) where T : class
    {
        if (node is Leaf<T> leafNode)
        {
            // Crear un nodo hoja del ViewModel
            return new ClusterNode(leafNode.Item, parent);
        }
        else if (node is Internal<T> internalNode)
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