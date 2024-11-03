using Zafiro.Tables;

namespace Zafiro.DataAnalysis.Clustering;

public interface IClusteringStrategy<T>
{
    ClusterNode<T> Clusterize(Table<ClusterNode<T>, double> table);
}