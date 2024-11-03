namespace Zafiro.DataAnalysis.Clustering;

public abstract class ClusterNode<T>;

public class LeafNode<T> : ClusterNode<T> where T : class
{
    public T Item { get; }

    public LeafNode(T item)
    {
        Item = item;
    }

    public override string? ToString()
    {
        return Item.ToString();
    }
}

public class InternalNode<T> : ClusterNode<T>
{
    public ClusterNode<T> Left { get; }
    public ClusterNode<T> Right { get; }
    public double MergeDistance { get; }

    public InternalNode(ClusterNode<T> left, ClusterNode<T> right, double mergeDistance)
    {
        Left = left;
        Right = right;
        MergeDistance = mergeDistance;
    }
}
