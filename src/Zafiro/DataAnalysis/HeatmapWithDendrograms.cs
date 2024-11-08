using System.Linq;
using MoreLinq;
using Zafiro.DataAnalysis.Clustering;
using Zafiro.Tables;

namespace Zafiro.DataAnalysis;

public class HeatmapWithDendrograms(ICluster rowsCluster, ICluster columnsCluster, ITable table) : IHeatmapWithDendrograms
{
    public ICluster RowsCluster { get; } = rowsCluster;
    public ICluster ColumnsCluster { get; } = columnsCluster;
    public ITable Table { get; } = table;

    public static IHeatmapWithDendrograms Create<TRow, TColumn>(Table<TRow, TColumn, double> table, IClusteringStrategy<TRow> rowClusteringStrategy, IClusteringStrategy<TColumn> columnClusteringStrategy) where TColumn : class where TRow : class
    {
        var columnDistances = table.ToColumnDistances();
        var clusterTableColumns = columnDistances.ToClusterTable();
        var columnsCluster = columnClusteringStrategy.Clusterize(clusterTableColumns);
        var columnOrder = MoreEnumerable.TraverseDepthFirst(columnsCluster,
        node => node is Internal<TColumn> i ? new[] {i.Left, i.Right} : Enumerable.Empty<Cluster<TColumn>>())
        .OfType<Leaf<TColumn>>()
        .Select(x => x.Item)
        .ToList();

        var rowDistances = table.ToRowDistances();
        var clusterTableRows = rowDistances.ToClusterTable();
        var rowsCluster = rowClusteringStrategy.Clusterize(clusterTableRows);
        var rowOrder = MoreEnumerable.TraverseDepthFirst(rowsCluster,
                node => node is Internal<TRow> i ? new[] {i.Left, i.Right} : Enumerable.Empty<Cluster<TRow>>())
            .OfType<Leaf<TRow>>()
            .Select(x => x.Item)
            .ToList();

        var reorderedTable = table
            .ReorderColumns(columnOrder)
            .ReorderRows(rowOrder);

        var rowsClusterNode = ClusterNode.Create(rowsCluster);
        var columnsClusterNode = ClusterNode.Create(columnsCluster);
        return new HeatmapWithDendrograms(rowsClusterNode, columnsClusterNode, reorderedTable);
    }
}