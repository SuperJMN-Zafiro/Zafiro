using System.Linq;
using Zafiro.Tables;

namespace Zafiro.DataAnalysis.Clustering;

public static class Extensions
{
    public static Table<ClusterNode<TLabel>, double> ToClusterTable<TLabel>(this Table<TLabel, double> table) where TLabel : class
    {
        var clusters = table.RowLabels.Select(s => new LeafNode<TLabel>(s)).Cast<ClusterNode<TLabel>>();
        return new Table<ClusterNode<TLabel>, double>(table.Matrix, clusters.ToList());
    }
}