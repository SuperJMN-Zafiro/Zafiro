namespace Zafiro.DataAnalysis.Graphs;

public interface IEngine
{
    void Step();
    void Distribute(double width, double height);
}