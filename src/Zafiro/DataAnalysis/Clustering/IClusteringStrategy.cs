using Zafiro.Tables;

namespace Zafiro.DataAnalysis.Clustering;

public interface IClusteringStrategy<T>
{
    Cluster<T> Clusterize(Table<Cluster<T>, double> table);
}