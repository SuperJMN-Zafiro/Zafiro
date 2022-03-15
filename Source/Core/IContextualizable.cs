namespace Core
{
    public interface IContextualizable
    {
        void SetContext(object o);
        object Object { get; }
    }
}