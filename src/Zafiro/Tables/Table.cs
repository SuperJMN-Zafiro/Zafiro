using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zafiro.Mixins;

namespace Zafiro.Tables;

public class Table
{
    public static Table<TItem, TValue> FromSubsets<TItem, TValue>(params (TItem a, TItem b, TValue value)[] distanceEntries)
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

public class Table<TLabel, TCell> : Table<TLabel, TLabel, TCell>
{
    public Table(TCell[,] matrix, IList<TLabel> labels) : base(matrix, labels, labels)
    {
    }
}

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
    }

    public TCell Get(TRow row, TColumn column)
    {
        return Matrix[RowLabels.IndexOf(row), ColumnLabels.IndexOf(column)];
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append("\t");
        stringBuilder.AppendLine(RowLabels.Join("\t"));

        for (int r = 0; r < this.RowLabels.Count; r++)
        {
            stringBuilder.Append(RowLabels[r]);

            for (int c = 0; c < this.ColumnLabels.Count; c++)
            {
                stringBuilder.Append(this.Matrix[r, c]);
                stringBuilder.Append("\t");
            }

            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString();
    }
}