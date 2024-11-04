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