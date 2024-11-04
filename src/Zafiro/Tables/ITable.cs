using System.Collections;
using System.Collections.Generic;

namespace Zafiro.Tables;

public interface ITable
{
    public object[,] Matrix { get; }
    public IEnumerable RowLabels { get; }
    public IEnumerable ColumnLabels { get; }
    public int Rows { get; }
    public int Columns { get; }
    public IEnumerable<ICell> Cells { get; }
}