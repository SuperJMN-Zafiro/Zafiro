namespace Zafiro
{
    public interface IContextualizable
    {
        void SetContext(object o);
        object Object { get; }
    }
}