namespace Zafiro.DataAnalysis.Graphs;

public interface IMutableNode
{
    public double X { get; set; }
    public double Y { get; set; }
    public double ForceX { get; set; }
    public double ForceY { get; set; }
    bool IsFrozen { get; set; }
    double Weight { get; }
}