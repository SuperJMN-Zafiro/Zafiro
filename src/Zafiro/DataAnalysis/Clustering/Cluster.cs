namespace Zafiro.DataAnalysis.Clustering;

public abstract class Cluster<T>;

public class Leaf<T> : Cluster<T> where T : class
{
    public Leaf(T item)
    {
        Item = item;
    }

    public T Item { get; }

    public override string? ToString()
    {
        return Item.ToString();
    }
}

public class Internal<T> : Cluster<T>
{
    public Cluster<T> Left { get; }
    public Cluster<T> Right { get; }
    public double MergeDistance { get; }

    public Internal(Cluster<T> left, Cluster<T> right, double mergeDistance)
    {
        Left = left;
        Right = right;
        MergeDistance = mergeDistance;
    }
}
