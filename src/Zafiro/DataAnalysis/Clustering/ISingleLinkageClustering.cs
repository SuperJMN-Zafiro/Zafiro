using Zafiro.Tables;

namespace Zafiro.DataAnalysis.Clustering;

public interface ISingleLinkageClustering<T>
{
    ClusterNode<T> Clusterize(Table<ClusterNode<T>, double> table);
}