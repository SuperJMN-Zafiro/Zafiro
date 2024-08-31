namespace Zafiro;

public interface IContextualizable
{
    object Object { get; }
    void SetContext(object o);
}