using System.Collections.Generic;

namespace Zafiro.DataAnalysis.Clustering;

public interface ICluster
{
    public object? Item { get; }
    public ICluster? Parent { get;}
    public ICluster? Left { get; }
    public ICluster? Right { get; }
    public double MergeDistance { get; }
    public IEnumerable<ICluster> Children { get; }
}