namespace Zafiro.DataAnalysis.Clustering;

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