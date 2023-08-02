namespace Zafiro.Zafiro
{
    public class Association
    {
        public object Parent { get; }
        public object Child { get; }

        public Association(object parent, object child)
        {
            Parent = parent;
            Child = child;
        }
    }
}