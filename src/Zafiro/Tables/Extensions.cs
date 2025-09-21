using System;
using System.Collections.Generic;
using System.Linq;

namespace Zafiro.Tables;

public static class Extensions
{
    public static Table<TItem, TValue> ToTable<TItem, TValue>(this IEnumerable<(TItem a, TItem b, TValue value)> distanceEntries)
        where TItem : notnull where TValue : notnull
    {
        return ToTable(distanceEntries.ToArray());
    }

    public static Table<TItem, TValue> ToTable<TItem, TValue>(params (TItem a, TItem b, TValue value)[] distanceEntries) where TItem : notnull where TValue : notnull
    {
        // Recopilar todas las etiquetas únicas
        var labels = distanceEntries
            .SelectMany(entry => new[] { entry.a, entry.b })
            .Distinct()
            .ToList();

        // Crear un diccionario para mapear etiquetas a índices
        var labelIndices = labels
            .Select((label, index) => new { label, index })
            .ToDictionary(x => x.label, x => x.index);

        int size = labels.Count;
        var matrix = new TValue[size, size];

        // Crear un diccionario para acceso rápido a los valores
        var valueDictionary = new Dictionary<(TItem, TItem), TValue>();

        foreach (var entry in distanceEntries)
        {
            valueDictionary[(entry.a, entry.b)] = entry.value;
            valueDictionary[(entry.b, entry.a)] = entry.value; // Añadir ambos órdenes si es simétrico
        }

        // Rellenar la matriz
        for (int i = 0; i < size; i++)
        {
            var labelI = labels[i];

            for (int j = 0; j < size; j++)
            {
                var labelJ = labels[j];

                if (labelI.Equals(labelJ))
                {
                    // Asignar el valor por defecto en la diagonal
                    matrix[i, j] = default;
                }
                else if (valueDictionary.TryGetValue((labelI, labelJ), out TValue value))
                {
                    matrix[i, j] = value;
                }
                else
                {
                    // Manejar valores faltantes según sea necesario
                    matrix[i, j] = default;
                }
            }
        }

        return new Table<TItem, TValue>(matrix, labels);
    }

    public static TaggedEnumerable<TCell, TRow> GetRow<TRow, TColumn, TCell>(this Table<TRow, TColumn, TCell> table,
        TRow row) where TCell : notnull where TRow : notnull
    {
        return new TaggedEnumerable<TCell, TRow>(row, table.ColumnLabels.Select(column => table.Get(row, column)));
    }

    public static TaggedEnumerable<TCell, TColumn> GetColumn<TRow, TColumn, TCell>(
        this Table<TRow, TColumn, TCell> table, TColumn column) where TColumn : notnull where TCell : notnull
    {
        return new TaggedEnumerable<TCell, TColumn>(column, table.RowLabels.Select(row => table.Get(row, column)));
    }

    public static IEnumerable<TaggedEnumerable<TCell, TColumn>> GetColumns<TRow, TColumn, TCell>(
        this Table<TRow, TColumn, TCell> table) where TCell : notnull where TColumn : notnull
    {
        return table.ColumnLabels.Select(table.GetColumn);
    }

    public static IEnumerable<TaggedEnumerable<TCell, TRow>> GetRows<TRow, TColumn, TCell>(
        this Table<TRow, TColumn, TCell> table) where TCell : notnull where TRow : notnull
    {
        return table.RowLabels.Select(table.GetRow);
    }

    public static Table<TRow, TResult> ApplyRowPairOperation<TRow, TColumn, TCell, TResult>(
        this Table<TRow, TColumn, TCell> table,
        Func<TaggedEnumerable<TCell, TRow>, TaggedEnumerable<TCell, TRow>, TResult> func) where TResult : notnull where TCell : notnull where TRow : notnull
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
        Func<TaggedEnumerable<TCell, TColumn>, TaggedEnumerable<TCell, TColumn>, TResult> func) where TResult : notnull where TCell : notnull where TColumn : notnull
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

    public static Table<TRow, double> ToRowDistances<TRow, TColumn>(this Table<TRow, TColumn, double> table) where TRow : notnull
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

    public static Table<TColumn, double> ToColumnDistances<TRow, TColumn>(this Table<TRow, TColumn, double> table) where TColumn : notnull
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
        IList<TRow> newRowOrder) where TCell : notnull
    {
        var matrix = new TCell[table.Rows, table.Columns];

        for (var r = 0; r < table.Rows; r++)
        {
            for (var c = 0; c < table.Columns; c++)
            {
                matrix[r, c] = table.Get(newRowOrder[r], table.ColumnLabels[c]);
            }
        }

        return new Table<TRow, TColumn, TCell>(matrix, newRowOrder, table.ColumnLabels);
    }

    public static Table<TRow, TColumn, TCell> ReorderColumns<TRow, TColumn, TCell>(
        this Table<TRow, TColumn, TCell> table,
        IList<TColumn> newColumnOrder) where TCell : notnull
    {
        var matrix = new TCell[table.Rows, table.Columns];

        for (var r = 0; r < table.Rows; r++)
        {
            for (var c = 0; c < table.Columns; c++)
            {
                matrix[r, c] = table.Get(table.RowLabels[r], newColumnOrder[c]);
            }
        }

        return new Table<TRow, TColumn, TCell>(matrix, table.RowLabels, newColumnOrder);
    }

    public static IEnumerable<TCell> Items<TRow, TColumn, TCell>(this Table<TRow, TColumn, TCell> table)
    {
        for (var i = 0; i < table.Rows; i++)
        {
            for (var c = 0; c < table.Columns; c++)
            {
                yield return table.Matrix[i, c];
            }
        }
    }
}