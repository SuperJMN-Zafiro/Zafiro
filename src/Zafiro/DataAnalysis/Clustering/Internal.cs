namespace Zafiro.DataAnalysis.Clustering;

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