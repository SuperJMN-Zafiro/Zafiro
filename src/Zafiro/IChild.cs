namespace Zafiro
{
    public interface IChild<TParent>
    {
        TParent Parent { get; set; }
    }
}