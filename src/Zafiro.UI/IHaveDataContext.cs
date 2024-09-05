namespace Zafiro.UI;

public interface IHaveDataContext
{
    object Object { get; }
    void SetDataContext(object dataContext);
}