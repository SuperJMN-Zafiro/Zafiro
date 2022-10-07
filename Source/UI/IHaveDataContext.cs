namespace Zafiro.UI
{
    public interface IHaveDataContext
    {
        void SetDataContext(object dataContext);
        object Object { get; }
    }
}