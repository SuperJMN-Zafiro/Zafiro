namespace Zafiro.DataAnalysis.Graphs;

public interface IEdge<out T>
{
    public T From { get; }
    public T To { get; }
}