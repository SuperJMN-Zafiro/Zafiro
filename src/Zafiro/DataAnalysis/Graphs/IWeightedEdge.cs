namespace Zafiro.DataAnalysis.Graphs;

public interface IWeightedEdge<out T> : IEdge<T>
{
    public double Weight { get; }
}