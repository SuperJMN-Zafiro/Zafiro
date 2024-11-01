namespace Zafiro.Clustering;

// Clase base genérica
public abstract class ClusterNode<T>
{
    // No incluye MergeDistance, ya que solo aplica a InternalNode
}

// Nodo hoja, que contiene el "item" de tipo T
public class LeafNode<T> : ClusterNode<T>
{
    public T Item { get; }

    public LeafNode(T item)
    {
        Item = item;
    }
}

// Nodo interno que representa una fusión
public class InternalNode<T> : ClusterNode<T>
{
    public ClusterNode<T> Left { get; }
    public ClusterNode<T> Right { get; }
    public double MergeDistance { get; } // Solo en nodos internos

    public InternalNode(ClusterNode<T> left, ClusterNode<T> right, double mergeDistance)
    {
        Left = left;
        Right = right;
        MergeDistance = mergeDistance;
    }
}
