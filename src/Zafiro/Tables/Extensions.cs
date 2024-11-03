using System;
using System.Collections.Generic;
using System.Linq;

namespace Zafiro.Tables;

public static class Extensions
{
    public static TaggedEnumerable<TCell, TRow> GetRow<TRow, TColumn, TCell>(this Table<TRow, TColumn, TCell> table,
        TRow row)
    {
        return new TaggedEnumerable<TCell, TRow>(row, table.ColumnLabels.Select(column => table.Get(row, column)));
    }

    public static TaggedEnumerable<TCell, TColumn> GetColumn<TRow, TColumn, TCell>(
        this Table<TRow, TColumn, TCell> table, TColumn column)
    {
        return new TaggedEnumerable<TCell, TColumn>(column, table.RowLabels.Select(row => table.Get(row, column)));
    }

    public static IEnumerable<TaggedEnumerable<TCell, TColumn>> GetColumns<TRow, TColumn, TCell>(
        this Table<TRow, TColumn, TCell> table)
    {
        return table.ColumnLabels.Select(table.GetColumn);
    }

    public static IEnumerable<TaggedEnumerable<TCell, TRow>> GetRows<TRow, TColumn, TCell>(
        this Table<TRow, TColumn, TCell> table)
    {
        return table.RowLabels.Select(table.GetRow);
    }

    public static Table<TRow, TResult> ApplyRowPairOperation<TRow, TColumn, TCell, TResult>(
        this Table<TRow, TColumn, TCell> table,
        Func<TaggedEnumerable<TCell, TRow>, TaggedEnumerable<TCell, TRow>, TResult> func)
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

    public static Table<TColumn, TResult> ApplyColumnPairOperation<TRow, TColumn, TCell, TResult>(
        this Table<TRow, TColumn, TCell> table,
        Func<TaggedEnumerable<TCell, TColumn>, TaggedEnumerable<TCell, TColumn>, TResult> func)
    {
        var columnCount = table.ColumnLabels.Count;
        var matrix = new TResult[columnCount, columnCount];

        var columns = table.GetColumns().ToList();

        for (var i = 0; i < columnCount; i++)
        {
            for (var j = 0; j < columnCount; j++)
            {
                var columnOne = columns[i];
                var columnTwo = columns[j];

                var result = func(columnOne, columnTwo);

                matrix[i, j] = result;
            }
        }

        return new Table<TColumn, TResult>(matrix, table.ColumnLabels);
    }

    public static Table<TRow, double> ToRowDistances<TRow, TColumn>(this Table<TRow, TColumn, double> table)
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

    public static Table<TColumn, double> ToColumnDistances<TRow, TColumn>(this Table<TRow, TColumn, double> table)
    {
        return table.ApplyColumnPairOperation((a, b) =>
        {
            var diffs = a.Zip(b, (x, y) => x - y);
            var diffsSquared = diffs.Select(i => Math.Pow(i, 2));
            var sumOfDiffsSquared = diffsSquared.Sum();

            var sqrt = Math.Sqrt(sumOfDiffsSquared);
            return sqrt;
        });
    }

    public static Table<TRow, TColumn, TCell> ReorderRows<TRow, TColumn, TCell>(
        this Table<TRow, TColumn, TCell> table,
        IList<TRow> newRowOrder)
    {
        var matrix = new TCell[table.Height, table.Width];

        for (var r = 0; r < table.Height; r++)
        {
            for (var c = 0; c < table.Width; c++)
            {
                matrix[r, c] = table.Get(newRowOrder[r], table.ColumnLabels[c]);
            }
        }

        return new Table<TRow, TColumn, TCell>(matrix, newRowOrder, table.ColumnLabels);
    }

    public static Table<TRow, TColumn, TCell> ReorderColumns<TRow, TColumn, TCell>(
        this Table<TRow, TColumn, TCell> table,
        IList<TColumn> newColumnOrder)
    {
        var matrix = new TCell[table.Height, table.Width];

        for (var r = 0; r < table.Height; r++)
        {
            for (var c = 0; c < table.Width; c++)
            {
                matrix[r, c] = table.Get(table.RowLabels[r], newColumnOrder[c]);
            }
        }

        return new Table<TRow, TColumn, TCell>(matrix, table.RowLabels, newColumnOrder);
    }
}