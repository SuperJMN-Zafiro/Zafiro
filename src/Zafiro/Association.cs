namespace Zafiro;

public class Association
{
    public Association(object parent, object child)
    {
        Parent = parent;
        Child = child;
    }

    public object Parent { get; }
    public object Child { get; }
}