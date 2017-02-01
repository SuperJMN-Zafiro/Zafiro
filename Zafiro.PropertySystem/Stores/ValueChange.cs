namespace Zafiro.PropertySystem.Stores
{
    public class ValueChange
    {
        public ValueChange(object previous, object @new)
        {
            Previous = previous;
            New = @new;
        }

        public object Previous { get;  }
        public object New { get; }
    }
}