using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq.Extensions;

namespace Zafiro.Tables;

public class Table<TLabel, TCell>(TCell[,] matrix, IList<TLabel> labels) : Table<TLabel, TLabel, TCell>(matrix, labels, labels) where TCell : notnull;

public class Table<TRow, TColumn, TCell> : ITable where TCell: notnull
{
    public TCell[,] Matrix { get; }
    object[,] ITable.Matrix => GetObjectMatrix();

    IEnumerable ITable.ColumnLabels => ColumnLabels;
    IEnumerable ITable.RowLabels => RowLabels;

    private object[,] GetObjectMatrix()
    {
        var objectMatrix = new object[Rows, Columns];
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                objectMatrix[i, j] = Matrix[i, j];
            }
        }
        return objectMatrix;
    }

    public IList<TRow> RowLabels { get; }
    public IList<TColumn> ColumnLabels { get; }

    public Table(TCell[,] matrix, IList<TRow> rowLabels, IList<TColumn> columnLabels)
    {
        var rowCount = matrix.GetLength(0);
        if (rowCount != rowLabels.Count)
        {
            throw new InvalidOperationException($"Invalid row count. Matrix has a dimension of {rowCount}, but {rowLabels} has {rowLabels.Count} items");
        }

        var columnCount = matrix.GetLength(1);
        if (columnCount != columnLabels.Count)
        {
            throw new InvalidOperationException($"Invalid row count. Matrix has a dimension of {columnCount}, but {columnLabels} has {columnLabels.Count} items");
        }

        Matrix = matrix;
        RowLabels = rowLabels;
        ColumnLabels = columnLabels;
        Columns = ColumnLabels.Count;
        Rows = RowLabels.Count;
    }

    public TCell Get(TRow row, TColumn column)
    {
        return Matrix[RowLabels.IndexOf(row), ColumnLabels.IndexOf(column)];
    }

    public int Columns { get; }

    public IEnumerable<ICell> Cells => this.GetRows().Select((row, i) => (row, i)).Cartesian(
        this.GetColumns().Select((column, i) => (column, i)),
        (row, column) =>
        {
            var rowTag = row.row.Tag;
            var columnTag = column.column.Tag;
            return new Cell<TRow, TColumn, TCell>(row.i, column.i, rowTag, columnTag, Get(rowTag, columnTag));
        });

    public int Rows { get; }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        int colCount = this.ColumnLabels.Count;
        int rowCount = this.RowLabels.Count;

        // Arreglo para almacenar el ancho m�ximo de cada columna (incluyendo la columna de etiquetas de fila)
        int[] maxWidths = new int[colCount + 1];

        // Calcular el ancho m�ximo para la columna de etiquetas de fila
        maxWidths[0] = this.RowLabels.Max(label => label?.ToString().Length ?? 0);

        // Calcular el ancho m�ximo para cada columna
        for (int c = 0; c < colCount; c++)
        {
            int max = this.ColumnLabels[c]?.ToString().Length ?? 0;
            for (int r = 0; r < rowCount; r++)
            {
                int cellLength = this.Matrix[r, c]?.ToString().Length ?? 0;
                if (cellLength > max)
                {
                    max = cellLength;
                }
            }
            maxWidths[c + 1] = max;
        }

        // Construir la cabecera de la tabla
        stringBuilder.Append("".PadRight(maxWidths[0] + 2)); // Espacio para la esquina superior izquierda
        for (int c = 0; c < colCount; c++)
        {
            stringBuilder.Append(this.ColumnLabels[c]?.ToString().PadRight(maxWidths[c + 1] + 2));
        }
        stringBuilder.AppendLine();

        // Construir las filas de la tabla
        for (int r = 0; r < rowCount; r++)
        {
            stringBuilder.Append(this.RowLabels[r]?.ToString().PadRight(maxWidths[0] + 2));
            for (int c = 0; c < colCount; c++)
            {
                string cellValue = this.Matrix[r, c]?.ToString() ?? "";
                stringBuilder.Append(cellValue.PadRight(maxWidths[c + 1] + 2));
            }
            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }
}