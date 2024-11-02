using System;
using System.Collections.Generic;
using System.Linq;
using Zafiro.Clustering;

namespace Zafiro.Tables;

public static class Extensions
{
    public static TaggedEnumerable<TCell, TRow> GetRow<TRow, TColumn, TCell>(this Table<TRow, TColumn, TCell> table, TRow row)
    {
        return new TaggedEnumerable<TCell, TRow>(row, table.ColumnLabels.Select(column => table.Get(row, column)));
    }

    public static TaggedEnumerable<TCell, TColumn> GetColumn<TRow, TColumn, TCell>(this Table<TRow, TColumn, TCell> table, TColumn column)
    {
        return new TaggedEnumerable<TCell, TColumn>(column, table.RowLabels.Select(row => table.Get(row, column)));
    }

    public static IEnumerable<TaggedEnumerable<TCell, TColumn>> GetColumns<TRow, TColumn, TCell>(this Table<TRow, TColumn, TCell> table)
    {
        return table.ColumnLabels.Select(table.GetColumn);
    }

    public static IEnumerable<TaggedEnumerable<TCell, TRow>> GetRows<TRow, TColumn, TCell>(this Table<TRow, TColumn, TCell> table)
    {
        return table.RowLabels.Select(table.GetRow);
    }

    public static Table<TRow, TResult> ApplyRowPairOperation<TRow, TColumn, TCell, TResult>(this Table<TRow, TColumn, TCell> table, Func<TaggedEnumerable<TCell, TRow>, TaggedEnumerable<TCell, TRow>, TResult> func)
    {
        var rowCount = table.RowLabels.Count;
        var matrix = new TResult[rowCount, rowCount];

        var rows = table.GetRows().ToList();

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

        return new Table<TRow, TResult>(matrix, table.RowLabels);
    }

    public static Table<TRow, TRow, double> RowDistanceMatrix<TRow, TColumn>(this Table<TRow, TColumn, double> table)
    {
        return table.ApplyRowPairOperation((a, b) =>
        {
            var sum = a.Zip(b, (x, y) => Math.Pow(x - y, 2)).Sum();
            var sqrt = Math.Sqrt(sum);
            return sqrt;
        });
    }

    public static Table<TRow, double> ToRowDistances<TRow, TColumn>(this Table<TRow, TColumn, int> table)
    {
        return table.ApplyRowPairOperation((a, b) =>
        {
            var diffs = a.Zip(b, (x, y) => x - y);
            var diffsSquared = diffs.Select(i => Math.Pow(i, 2));
            var sumOfDiffsSquared = diffsSquared.Sum();

            var sqrt = Math.Sqrt(sumOfDiffsSquared);
            return sqrt;
        });
    }

    public static Table<ClusterNode<TLabel>, double> ToClusterTable<TLabel>(this Table<TLabel, double> table)
    {
        var clusters = table.RowLabels.Select(s => new LeafNode<TLabel>(s)).Cast<ClusterNode<TLabel>>();
        var clusterNodeBases = clusters.ToList();
        return new Table<ClusterNode<TLabel>, double>(table.Matrix, clusterNodeBases);
    }
}