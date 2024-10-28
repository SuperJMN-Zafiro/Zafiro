namespace Zafiro.Graphs;

public interface IWeightedEdge<out T> : IEdge<T>
{
    public double Weight { get; }
}