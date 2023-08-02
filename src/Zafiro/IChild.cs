namespace Zafiro.Core
{
    public interface IChild<TParent>
    {
        TParent Parent { get; set; }
    }
}