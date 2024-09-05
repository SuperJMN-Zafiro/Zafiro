namespace Zafiro.UI;

public interface IResult<T>
{
    public Task<T> Result { get; }
}