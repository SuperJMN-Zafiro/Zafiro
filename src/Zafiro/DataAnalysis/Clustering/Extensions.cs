using System.Linq;
using Zafiro.Tables;

namespace Zafiro.DataAnalysis.Clustering;

public static class Extensions
{
    public static Table<Cluster<TLabel>, double> ToClusterTable<TLabel>(this Table<TLabel, double> table) where TLabel : class
    {
        var clusters = table.RowLabels.Select(s => new Leaf<TLabel>(s)).Cast<Cluster<TLabel>>();
        return new Table<Cluster<TLabel>, double>(table.Matrix, clusters.ToList());
    }
}