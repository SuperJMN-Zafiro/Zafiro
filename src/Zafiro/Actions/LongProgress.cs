namespace Zafiro.Actions;

public record LongProgress : IProgress
{
    public LongProgress(long current, long total)
    {
        Current = current;
        Total = total;
    }

    public LongProgress()
    {
    }

    public long Current { get; }
    public long Total { get; }
    public double Value => Total == 0 ? 0 : (double)Current / Total;

    public static LongProgress operator +(LongProgress left, LongProgress right)
    {
        return new LongProgress(left.Current + right.Current, left.Total + right.Total);
    }
}