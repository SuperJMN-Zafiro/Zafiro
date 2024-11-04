using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using ReactiveUI;

namespace Zafiro.Tables;

public class Cell
{
    public int RowIndex { get; }
    public int ColumIndex { get; }
    public object Row { get; }
    public object Column { get; }
    public object Item { get; }

    public Cell(int rowIndex, int columIndex, object row, object column, object item)
    {
        RowIndex = rowIndex;
        ColumIndex = columIndex;
        Row = row;
        Column = column;
        Item = item;
    }
}

public class DoubleCell : Cell
{
    public new double Item { get; }
    public double Max { get; }

    public DoubleCell(int rowIndex, int columnIndex, object rowTag, object columnTag, double item, double max)
        : base(rowIndex, columnIndex, rowTag, columnTag, item)
    {
        Item = item;
        Normalized = item / max;
        Max = max;
    }

    public double Normalized { get; }
}

public class DoubleTable : ObjectTable
{
    public override IEnumerable<DoubleCell> Cells { get; }

    public DoubleTable(Table<object, object, double> table) : base(ConvertTable(table))
    {
        Cells = table.GetRows().Select((row, i) => (row, i)).Cartesian(
            table.GetColumns().Select((column, i) => (column, i)),
            (row, column) =>
            {
                var rowTag = row.row.Tag;
                var columnTag = column.column.Tag;
                var value = table.Get(rowTag, columnTag);
                var max = table.Items().Max();
                return new DoubleCell(row.i, column.i, rowTag, columnTag, value, max);
            });
    }

    // Método para convertir la tabla genérica a una de objetos para el constructor base
    private static Table<object, object, object> ConvertTable(Table<object, object, double> table)
    {
        var matrix = new object[table.Rows, table.Columns];

        for (var r = 0; r < table.Rows; r++)
        for (var c = 0; c < table.Columns; c++)
            matrix[r, c] = table.Matrix[r, c];

        return new Table<object, object, object>(
            matrix,
            table.RowLabels,
            table.ColumnLabels);
    }

    public static new DoubleTable Create<TRow, TColumn>(Table<TRow, TColumn, double> table)
    {
        var objectTable = new Table<object, object, double>(
            table.Matrix,
            table.RowLabels.Cast<object>().ToList(),
            table.ColumnLabels.Cast<object>().ToList());

        return new DoubleTable(objectTable);
    }
}
