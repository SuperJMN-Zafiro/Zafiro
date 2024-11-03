using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zafiro.Tables;

public class Table
{
    public static Table<TItem, TValue> FromSubsets<TItem, TValue>(params (TItem a, TItem b, TValue value)[] distanceEntries) where TItem : notnull
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
}

public class Table<TLabel, TCell>(TCell[,] matrix, IList<TLabel> labels) : Table<TLabel, TLabel, TCell>(matrix, labels, labels);

public class Table<TRow, TColumn, TCell>
{
    public TCell[,] Matrix { get; }
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
        Width = ColumnLabels.Count;
        Height = RowLabels.Count;
    }

    public TCell Get(TRow row, TColumn column)
    {
        return Matrix[RowLabels.IndexOf(row), ColumnLabels.IndexOf(column)];
    }

    public int Width { get; }

    public int Height { get; }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        int colCount = this.ColumnLabels.Count;
        int rowCount = this.RowLabels.Count;

        // Arreglo para almacenar el ancho máximo de cada columna (incluyendo la columna de etiquetas de fila)
        int[] maxWidths = new int[colCount + 1];

        // Calcular el ancho máximo para la columna de etiquetas de fila
        maxWidths[0] = this.RowLabels.Max(label => label?.ToString().Length ?? 0);

        // Calcular el ancho máximo para cada columna
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