using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Zafiro.Tables;

public class ObjectTable
{
    public ObjectTable(Table<object, object, object> table)
    {
        Cells = table.GetRows().Select((row, i) => (row, i)).Cartesian(
            table.GetColumns().Select((column, i) => (column, i)),
            (row, column) =>
            {
                var rowTag = row.row.Tag;
                var columnTag = column.column.Tag;
                return new Cell(row.i, column.i, rowTag, columnTag, table.Get(rowTag, columnTag));
            });

        Rows = table.Rows;
        Columns = table.Columns;
        RowLabels = table.RowLabels;
        ColumnLabels = table.ColumnLabels;
    }

    public IEnumerable<object> ColumnLabels { get; }
    public virtual IEnumerable<Cell> Cells { get; }
    public IEnumerable<object> RowLabels { get; }
    public int Columns { get; }
    public int Rows { get; }

    public static ObjectTable Create<TRow, TColumn, TCell>(Table<TRow, TColumn, TCell> table) where TCell : notnull
    {
        var matrix = new object[table.Rows, table.Columns];

        for (var r = 0; r < table.Rows; r++)
        for (var c = 0; c < table.Columns; c++)
            matrix[r, c] = table.Matrix[r, c];

        var objectTable = new Table<object, object, object>(matrix, table.RowLabels.Cast<object>().ToList(),
            table.ColumnLabels.Cast<object>().ToList());
        return new ObjectTable(objectTable);
    }
}