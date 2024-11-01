using System;
using System.Collections.Generic;
using System.Linq;
using Zafiro.Clustering;

namespace Zafiro.Clustering;

public static class Extensions
{
    public static TaggedEnumerable<TCell, TRow> GetRow<TRow, TColumn, TCell>(this LabeledTable<TRow, TColumn, TCell> labeledTable, TRow row)
    {
        return new TaggedEnumerable<TCell, TRow>(row, labeledTable.ColumnLabels.Select(column => labeledTable.Get(row, column)));
    }

    public static TaggedEnumerable<TCell, TColumn> GetColumn<TRow, TColumn, TCell>(this LabeledTable<TRow, TColumn, TCell> labeledTable, TColumn column)
    {
        return new TaggedEnumerable<TCell, TColumn>(column, labeledTable.RowLabels.Select(row => labeledTable.Get(row, column)));
    }

    public static IEnumerable<TaggedEnumerable<TCell, TColumn>> GetColumns<TRow, TColumn, TCell>(this LabeledTable<TRow, TColumn, TCell> labeledTable)
    {
        return labeledTable.ColumnLabels.Select(labeledTable.GetColumn);
    }

    public static IEnumerable<TaggedEnumerable<TCell, TRow>> GetRows<TRow, TColumn, TCell>(this LabeledTable<TRow, TColumn, TCell> labeledTable)
    {
        return labeledTable.RowLabels.Select(labeledTable.GetRow);
    }

    public static Table<TRow, TResult> ApplyRowPairOperation<TRow, TColumn, TCell, TResult>(this LabeledTable<TRow, TColumn, TCell> labeledTable, Func<TaggedEnumerable<TCell, TRow>, TaggedEnumerable<TCell, TRow>, TResult> func)
    {
        var rowCount = labeledTable.RowLabels.Count;
        var matrix = new TResult[rowCount, rowCount];

        var rows = labeledTable.GetRows().ToList();

        for (var i = 0; i < rowCount; i++)
        {
            for (var j = 0; j < rowCount; j++)
            {
                var rowOne = rows[i];
                var rowTwo = rows[j];

                var result = func(rowOne, rowTwo);

                matrix[i, j] = result;
            }
        }

        return new Table<TRow, TResult>(matrix, labeledTable.RowLabels);
    }

    public static LabeledTable<TRow, TRow, double> RowDistanceMatrix<TRow, TColumn>(this LabeledTable<TRow, TColumn, double> labeledTable)
    {
        return labeledTable.ApplyRowPairOperation((a, b) =>
        {
            var sum = a.Zip(b, (x, y) => Math.Pow(x - y, 2)).Sum();
            var sqrt = Math.Sqrt(sum);
            return sqrt;
        });
    }

    public static Table<TRow, double> ToRowDistances<TRow, TColumn>(this LabeledTable<TRow, TColumn, int> labeledTable)
    {
        return labeledTable.ApplyRowPairOperation((a, b) =>
        {
            var sum = a.Zip(b, (x, y) => Math.Pow(x - y, 2)).Sum();
            var sqrt = Math.Sqrt(sum);
            return sqrt;
        });
    }

    public static Table<ClusterNode<TLabel>, double> ToClusterTable<TLabel>(this Table<TLabel, double> table)
    {
        var rowDistances = table.RowDistanceMatrix();
        var clusters = rowDistances.RowLabels.Select(s => new LeafNode<TLabel>(s)).Cast<ClusterNode<TLabel>>();
        var clusterNodeBases = clusters.ToList();
        return new Table<ClusterNode<TLabel>, double>(rowDistances.Matrix, clusterNodeBases);
    }
}