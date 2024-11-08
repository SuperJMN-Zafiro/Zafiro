namespace Zafiro.Tables;

public interface ICell
{
    public object Item { get; }
    public object Row { get; }
    public object Column { get; }
    public int RowIndex { get; }
    public int ColumnIndex { get; }
}