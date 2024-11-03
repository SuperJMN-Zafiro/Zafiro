using System.Linq;
using MoreLinq;
using Zafiro.DataAnalysis.Clustering;
using Zafiro.Tables;

namespace Zafiro.DataAnalysis;

public class Heatmap<TRow, TColumn, TCell>(
    ClusterNode<TRow> rowsCluster,
    ClusterNode<TColumn> columnsCluster,
    Table<TRow, TColumn, TCell> table)
{
    public ClusterNode<TRow> RowsCluster { get; } = rowsCluster;
    public ClusterNode<TColumn> ColumnsCluster { get; } = columnsCluster;
    public Table<TRow, TColumn, TCell> Table { get; } = table;
}

public static class Heatmap
{
  public static Heatmap<TRow, TColumn, double> Create<TRow, TColumn>(Table<TRow, TColumn, double> table, IClusteringStrategy<TRow> rowClusteringStrategy, IClusteringStrategy<TColumn> columnClusteringStrategy) where TRow : class where TColumn : class
  {
        var columnDistances = table.ToColumnDistances();
        var clusterTableColumns = columnDistances.ToClusterTable();
        var columnsCluster = columnClusteringStrategy.Clusterize(clusterTableColumns);
        var columnOrder = MoreEnumerable.TraverseDepthFirst(columnsCluster,
                node => node is InternalNode<TColumn> i ? new[] {i.Left, i.Right} : Enumerable.Empty<ClusterNode<TColumn>>())
            .OfType<LeafNode<TColumn>>()
            .Select(x => x.Item)
            .ToList();

        var rowDistances = table.ToRowDistances();
        var clusterTableRows = rowDistances.ToClusterTable();
        var rowsCluster = rowClusteringStrategy.Clusterize(clusterTableRows);
        var rowOrder = MoreEnumerable.TraverseDepthFirst(rowsCluster,
                node => node is InternalNode<TRow> i ? new[] {i.Left, i.Right} : Enumerable.Empty<ClusterNode<TRow>>())
            .OfType<LeafNode<TRow>>()
            .Select(x => x.Item)
            .ToList();

        var reorderedTable = table
            .ReorderColumns(columnOrder)
            .ReorderRows(rowOrder);

        return new Heatmap<TRow, TColumn, double>(rowsCluster, columnsCluster, reorderedTable);
    }
}