namespace Zafiro.Tables;

public class Cell<TRow, TColumn, TItem>(int rowIndex, int columnIndex, TRow row, TColumn column, TItem item)
    : ICell
    where TRow : notnull
    where TColumn : notnull
    where TItem : notnull
{
    public int RowIndex { get; } = rowIndex;
    public int ColumnIndex { get; } = columnIndex;

    public TRow Row { get; } = row;
    public TColumn Column { get; } = column;
    public TItem Item { get; } = item;

    object ICell.Row => Row;
    object ICell.Column => Column;
    object ICell.Item => Item;
}
