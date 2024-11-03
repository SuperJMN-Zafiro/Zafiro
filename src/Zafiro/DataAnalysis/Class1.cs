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